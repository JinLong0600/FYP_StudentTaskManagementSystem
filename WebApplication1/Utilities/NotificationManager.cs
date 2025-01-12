using Microsoft.EntityFrameworkCore;
using StudentTaskManagement.Models;
using StudentTaskManagement.ViewModels;
using System.Text.Json;
using WebPush;

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
        private readonly IConfiguration _configuration;  // Add this

        public NotificationManager(
            StudentTaskManagementContext context,
            ILogger<NotificationManager> logger, IConfiguration configuration)
        {
            dbContext = context;
            _logger = logger;
        }

        
        public async Task RefreshNotificationQueueAsync()
        {
            // Step 1: Clear old notifications
            await ClearOldNotificationsAsync();

            // Step 2: Fetch tasks/subtasks that need notifications
            var tasks = await dbContext.L1Tasks
                .Include(t => t.L1NotificationPresets)
                .Where(t => t.L1NotificationPresets != null && t.DueDate > DateTime.Now)
                .ToListAsync();

            var subtasks = await dbContext.L1SubTasks
                .Include(t => t.L1NotificationPresets)
                .Where(t => t.L1NotificationPresets != null && t.DueDate > DateTime.Now)
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
                .GroupBy(n => new { n.UserId, n.NotificationType })
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
                        await SendChromeNotificationAsync(userGroup.Key.UserId, message);
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
                        userGroup.Key.UserId);
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
                Title = "Multiple Tasks Due",
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
                    .FirstOrDefaultAsync(s => s.UserId == userId);

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
                    "mailto:your-email@domain.com",  // Your contact email
                    _configuration["VAPID:PublicKey"],
                    _configuration["VAPID:PrivateKey"]
                );

                var webPushClient = new WebPushClient();
                await webPushClient.SendNotificationAsync(
                    new PushSubscription(pushEndpoint, p256dh, auth),
                    JsonSerializer.Serialize(notification),
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
            var scheduledTime = CalculateScheduledTime(
                task.L1NotificationPresets,
                task.DueDate);

            var queueItem = new NotificationQueues
            {
                TaskId = task.Id,
                UserId = task.CreatedByStudentId,
                NotificationPresetId = task.L1NotificationPresets.Id,
                Title = $"Task Due: {task.Title}",
                Message = $"Task '{task.Title}' is due at {task.DueDate:h:mm tt}",
                ScheduledTime = scheduledTime,
                NotificationType = task.L1NotificationPresets.Type,
                IsDaily = task.L1NotificationPresets.IsDaily,
                TaskDueDate = task.DueDate,
                CreatedAt = DateTime.Now,
                IsProcessed = false
            };

            await dbContext.NotificationQueues.AddAsync(queueItem);
        }

        private async Task CreateNotificationQueueItemForSubtask(L1SubTasks subtask)
        {
            var scheduledTime = CalculateScheduledTime(
                subtask.L1NotificationPresets,
                subtask.DueDate);

            var queueItem = new NotificationQueues
            {
                SubtaskId = subtask.Id,
                UserId = subtask.CreatedByStudentId,
                NotificationPresetId = subtask.L1NotificationPresets.Id,
                Title = $"subtask Due: {subtask.Title}",
                Message = $"subtask '{subtask.Title}' is due at {subtask.DueDate:h:mm tt}",
                ScheduledTime = scheduledTime,
                NotificationType = subtask.L1NotificationPresets.Type,
                IsDaily = subtask.L1NotificationPresets.IsDaily,
                TaskDueDate = subtask.DueDate,
                CreatedAt = DateTime.Now,
                IsProcessed = false
            };

            await dbContext.NotificationQueues.AddAsync(queueItem);
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

                    if (preset.IsDaily && reminderTime < DateTime.Now)
                    {
                        reminderTime = reminderTime.AddDays(1);
                    }

                    return reminderTime;

                default:
                    throw new ArgumentException("Invalid notification preset type");
            }
        }
    }
}
