using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StudentTaskManagement.Models;
using StudentTaskManagement.ViewModels;
using System.Text.Json;
using System.Threading.Tasks;
using WebPush;
using static StudentTaskManagement.Utilities.GeneralEnum;

namespace StudentTaskManagement.Utilities
{
    public interface INotificationManager
    {
        Task ProcessNotificationsAsync();
        Task RefreshNotificationQueueAsync();
    }

    public class NotificationManager : INotificationManager
    {
        private readonly StudentTaskManagementContext dbContext;
        private readonly ILogger<NotificationManager> _logger;
        private readonly VapidConfiguration _vapidConfig;

        public NotificationManager(
            StudentTaskManagementContext context,
            ILogger<NotificationManager> logger, IOptions<VapidConfiguration> vapidConfig)
        {
            dbContext = context;
            _logger = logger;
            _vapidConfig = vapidConfig.Value;
        }

        
        public async Task RefreshNotificationQueueAsync()
        {
            // Step 1: Clear old notifications
            await ClearOldNotificationsAsync();

            // Step 2: Fetch tasks/subtasks that need notifications
            var tasks = await dbContext.L1Tasks
                .Include(t => t.L1NotificationPresets)
                .Where(t => (t.IsNotification && t.L1NotificationPresets != null) && t.DueDate > DateTime.Now && t.Status != (int)ItemTaskStatus.Completed && t.Status != (int)ItemTaskStatus.Overdue)
                .ToListAsync();

            var subtasks = await dbContext.L1SubTasks
                .Include(t => t.L1NotificationPresets)
                .Where(t => (t.IsNotification && t.L1NotificationPresets != null) && t.DueDate > DateTime.Now && t.Status != (int)ItemTaskStatus.Completed && t.Status != (int)ItemTaskStatus.Overdue)
                .ToListAsync();

            // Step 3: Create notification queue items
            foreach (var task in tasks)
            {
                await CreateNotificationQueueItemForTask(task);
            }

            foreach (var subtask in subtasks)
            {
                await CreateNotificationQueueItemForSubtask(subtask);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task ProcessNotificationsAsync()
        {
            // Step 1: Get due notifications from queue
            var now = DateTime.Now;
            var pendingNotifications = await dbContext.NotificationQueues
                .Where(n => !n.IsProcessed
                    && n.ScheduledTime <= now
                    && (n.LastAttempt == null || n.LastAttempt < now.AddMinutes(-5)))
                .GroupBy(n => new { n.StudentId }) //, n.NotificationType })
                .ToListAsync();

            foreach (var userGroup in pendingNotifications)
            {
                try
                {
                    var notifications = userGroup.ToList();
                    var aggregatedMessages = AggregateNotifications(notifications);

                    // Step 2: Send notifications
                    foreach (var message in aggregatedMessages)
                    {
                        await SendChromeNotificationAsync(userGroup.Key.StudentId, message);
                    }

                    // Step 3: Mark as processed
                    foreach (var notification in notifications)
                    {
                        notification.IsProcessed = true;
                        notification.LastAttempt = now;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error processing notifications for user {UserId}",
                        userGroup.Key.StudentId);
                }
            }
            // Step 4: Save changes
            await dbContext.SaveChangesAsync();
        }
        
        private async Task ClearOldNotificationsAsync()
        {
            var oldNotifications = await dbContext.NotificationQueues
                .Where(n => n.IsProcessed || n.ScheduledTime.Date < DateTime.Today)
                .ToListAsync();

            dbContext.NotificationQueues.RemoveRange(oldNotifications);
            await dbContext.SaveChangesAsync();
        }

        private List<NotificationMessage> AggregateNotifications(
            List<NotificationQueues> notifications)
        {
            if (notifications.Count <= 3)
            {
                return notifications.Select(n => new NotificationMessage
                {
                    Title = n.Title,
                    Message = n.Message,
                    IsAggregated = false
                }).ToList();
            }

            var firstTask = notifications[0];
            var dueTime = firstTask.ScheduledTime.ToString("h:mm tt");

            return new List<NotificationMessage>
        {
            new NotificationMessage
            {
                Title = "Multiple Tasks Due.",
                Message = $"You have {notifications.Count} tasks due at {dueTime}: " +
                         $"{firstTask.Title} and {notifications.Count - 1} others",
                IsAggregated = true
            }
        };
        }

        private async Task SendChromeNotificationAsync(
            string userId,
            NotificationMessage message)
        {
            try
            {
                // Get user's subscription from database
                var pushSubscription = await dbContext.PushSubscriptions
                    .FirstOrDefaultAsync(s => s.StudentId == userId);

                if (pushSubscription == null)
                {
                    _logger.LogWarning($"No push subscription found for user {userId}");
                    return;
                }

                var notification = new
                {
                    title = message.Title,
                    body = message.Message,
                    icon = message.Icon,
                    requireInteraction = message.RequireInteraction,
                    data = message.Data ?? new Dictionary<string, string>()
                };

                var pushEndpoint = pushSubscription.Endpoint;
                var p256dh = pushSubscription.P256dh;
                var auth = pushSubscription.Auth;

                var vapidDetails = new VapidDetails(
                    "mailto:ajlong0600@gmail.com",
                    _vapidConfig.PublicKey,
                    _vapidConfig.PrivateKey
                );

                var webPushClient = new WebPushClient();
                await webPushClient.SendNotificationAsync(
                    new PushSubscription(pushEndpoint, p256dh, auth),
                    message.Message,
                    vapidDetails
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending push notification to user {userId}");
                throw;
            }
        }

        private async Task CreateNotificationQueueItemForTask(L1Tasks task)
        {
            var scheduledTime = new DateTime();
            
            if (task.L1NotificationPresets.IsSystemDefault) {
                scheduledTime = CalculateScheduledTimeForDefaultOption(task.DefaultNotificationOptions, task.DueDate);
            }
            else {
                scheduledTime = CalculateScheduledTime(task.L1NotificationPresets, task.DueDate);
            }
            if (scheduledTime.Date == DateTime.Now.Date)
            {
                var queueItem = new NotificationQueues
                {
                    L1TaskId = task.Id,
                    StudentId = task.CreatedByStudentId,
                    DefaultNotificationOption = task.L1NotificationPresets.IsSystemDefault ? task.DefaultNotificationOptions : null,
                    L1NotificationPresetId = !task.L1NotificationPresets.IsSystemDefault ? task.L1NotificationPresetId : null,
                    Title = $"Task Due: {task.Title}",
                    Message = $"Task '{task.Title}' is due at {scheduledTime:hh:mm tt}",
                    ScheduledTime = scheduledTime,
                    //NotificationType = task.L1NotificationPresets.Type,
                    //IsDaily = task.L1NotificationPresets.IsDaily,
                    //TaskDueDate = task.DueDate,
                    CreatedAt = DateTime.Now,
                    IsProcessed = false
                };
                await dbContext.NotificationQueues.AddAsync(queueItem);
            }

            
        }

        private async Task CreateNotificationQueueItemForSubtask(L1SubTasks subtask)
        {
            var scheduledTime = new DateTime();
            if (subtask.L1NotificationPresets.IsSystemDefault)
            {
                scheduledTime = CalculateScheduledTimeForDefaultOption(subtask.DefaultNotificationOptions, subtask.DueDate);
            }
            else
            {
                scheduledTime = CalculateScheduledTime(subtask.L1NotificationPresets, subtask.DueDate);
            }

            if (scheduledTime.Date == DateTime.Now.Date)
            {
                var queueItem = new NotificationQueues
                {
                    L1SubTasksId = subtask.Id,
                    StudentId = subtask.CreatedByStudentId,
                    L1NotificationPresetId = subtask.L1NotificationPresets.Id,
                    Title = $"Subtask Due: {subtask.Title}",
                    Message = $"Subtask '{subtask.Title}' is due at {subtask.DueDate:h:mm tt}",
                    ScheduledTime = scheduledTime,
                    //NotificationType = subtask.L1NotificationPresets.Type,
                    //IsDaily = subtask.L1NotificationPresets.IsDaily,
                    //TaskDueDate = subtask.DueDate,
                    CreatedAt = DateTime.Now,
                    IsProcessed = false
                };
                await dbContext.NotificationQueues.AddAsync(queueItem);
            }
        }

        private DateTime CalculateScheduledTime(
            L1NotificationPresets preset,
            DateTime taskDueDate)
        {
            switch (preset.Type)
            {
                case 1: // Days before at specific time
                    return taskDueDate
                        .AddDays(-preset.ReminderDaysBefore.GetValueOrDefault())
                        .Date
                        .Add(preset.ReminderTime?.TimeOfDay ?? DateTime.Now.TimeOfDay);

                case 2: // Hours/Minutes before
                    var reminderTime = taskDueDate
                        .AddHours(-preset.ReminderHoursBefore.GetValueOrDefault())
                        .AddMinutes(-preset.ReminderMinutesBefore.GetValueOrDefault());

                    return reminderTime;

                default:
                    throw new ArgumentException("Invalid notification preset type");
            }
        }

        private DateTime CalculateScheduledTimeForDefaultOption(int? defaultNotificationOptions, DateTime taskDueDate)
        {
            switch (defaultNotificationOptions)
            {
                case 3: // 1 day before
                    return taskDueDate.AddDays(-1);

                case 2: // 1 hour before
                    return taskDueDate.AddHours(-1);

                case 1: // 30 minutue before
                    return taskDueDate.AddMinutes(-30);

                default:
                    throw new ArgumentException("Invalid notification preset type");
            }
        }
    }
}
