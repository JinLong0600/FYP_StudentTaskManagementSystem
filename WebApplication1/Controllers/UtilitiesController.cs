using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentTaskManagement.Models;
using StudentTaskManagement.ViewModels;
using StudentTaskManagement.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using static StudentTaskManagement.Utilities.GeneralEnum;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Globalization;
using WebPush;
using Microsoft.Extensions.Options;

namespace StudentTaskManagement.Controllers
{
    public class UtilitiesController : Controller
    {
        private readonly StudentTaskManagementContext dbContext;
        private readonly ILogger<UtilitiesController> _logger;
        private readonly UserManager<L1Students> _userManager;
        private readonly INotificationManager _notificationManager;
        private readonly IConfiguration _configuration;  // Add this
        private readonly VapidConfiguration _vapidConfig;

        public UtilitiesController(StudentTaskManagementContext dbContext,
            ILogger<UtilitiesController> logger,
            UserManager<L1Students> userManager, INotificationManager notificationManager, IConfiguration configuration,
            IOptions<VapidConfiguration> vapidConfig)
        {
            this.dbContext = dbContext;
            this._logger = logger;
            _userManager = userManager;
            _notificationManager = notificationManager;
            _vapidConfig = vapidConfig.Value;
        }

        #region Recurs
        //[HttpPost]
        [HttpGet]
        [Route("api/utilities/check-overdue-tasks")]
        public async Task<IActionResult> CheckOverdueTasks()
        {
            try
            {
                // Get current date time
                var currentDateTime = DateTime.Now;

                // Find all tasks that are past their due date and not marked as overdue
                var overdueTasks = await dbContext.L1Tasks
                    .Where(t => t.DueDate < currentDateTime 
                        && t.Status != (int)ItemTaskStatus.Overdue
                        && t.Status != (int)ItemTaskStatus.Completed)
                    .ToListAsync();

                // Update task statuses
                foreach (var task in overdueTasks)
                {
                    task.Status = (int)ItemTaskStatus.Overdue;
                    task.LastModifiedDateTime = currentDateTime;
                }

                // Find all subtasks that are past their due date and not marked as overdue
                var overdueSubtasks = await dbContext.L1SubTasks
                    .Where(st => st.DueDate < currentDateTime 
                        && st.Status != (int)ItemTaskStatus.Overdue
                        && st.Status != (int)ItemTaskStatus.Completed)
                    .ToListAsync();

                // Update subtask statuses
                foreach (var subtask in overdueSubtasks)
                {
                    subtask.Status = (int)ItemTaskStatus.Overdue;
                    subtask.LastModifiedDateTime = currentDateTime;
                }

                // Save changes to database
                await dbContext.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = $"Updated {overdueTasks.Count} tasks and {overdueSubtasks.Count} subtasks to overdue status"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CheckOverdueTasks: {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "An error occurred while checking overdue tasks" });
            }
        }

        //[HttpPost]
        [HttpGet]
        [Route("api/utilities/generate-recurring-tasks")]
        public async Task<IActionResult> GenerateRecurringTasks()
        {
            try
            {
                var today = DateTime.Now.Date;
                
                // Get all active recurring tasks
                var recurringTasks = await dbContext.L1Tasks
                    .Include(t => t.L1SubTasks)
                    .Include(t => t.L1RecurringPatterns)
                    .Where(t => t.IsRecurring  && (t.IsParentRecurring == true || t.IsParentRecurring == null)
                        && t.L1RecurringPatterns.Status == (int)PresetPatternStatus.Active)
                    .ToListAsync();

                int tasksGenerated = 0;

                foreach (var task in recurringTasks)
                {
                    var preset = task.L1RecurringPatterns;
                    
                    // Skip if RecurringCount is reached
                    if (preset.RecurringCount.HasValue && 
                        task.GeneratedCount >= preset.RecurringCount)
                    {
                        continue;
                    }

                    // Check if we should generate a task today based on the preset type
                    if (ShouldGenerateTaskToday(task, preset, today))
                    {
                        await CreateNewTaskFromTemplate(task);
                        tasksGenerated++;

                        // Update the generated count on the original task
                        task.GeneratedCount = (task.GeneratedCount ?? 0) + 1;
                        task.LastModifiedDateTime = DateTime.Now;
                    }
                    else if (task.L1RecurringPatterns.IsSystemDefault && ShouldGenerateTaskTodayDefaultVersion(task, today))
                    {
                        await CreateNewTaskFromTemplate(task);
                        tasksGenerated++;

                        // Update the generated count on the original task
                        task.GeneratedCount = (task.GeneratedCount ?? 0) + 1;
                        task.LastModifiedDateTime = DateTime.Now;

                    }
                }

                await dbContext.SaveChangesAsync();

                return Ok(new { 
                    Success = true, 
                    Message = $"Generated {tasksGenerated} new tasks from recurring tasks" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GenerateRecurringTasks: {ex.Message}");
                return StatusCode(500, new { 
                    Success = false, 
                    Message = "An error occurred while generating recurring tasks" 
                });
            }
        }

        private bool ShouldGenerateTaskToday(L1Tasks task, L1RecurringPatterns preset, DateTime today)
        {
            switch (preset.Type)
            {
                case 1: // Day
                    if (preset.DaytoGenerate == "0") return true; // All days
                    // Split the DaytoGenerate string and check if today's day is in the list
                    var selectedDays = preset.DaytoGenerate.ToString().Split(',')
                                    .Select(int.Parse)
                                    .ToList();
                    return selectedDays.Contains((int)today.DayOfWeek + 1); // Adding 1 because DayOfWeek is 0-based

                case 2: // Week
                    if (preset.DaytoGenerate == "0")
                    {
                        // Recur on the same day as start date
                        var daysSinceStart = (today - task.StartDate.Date).Days;
                        return daysSinceStart > 0 && 
                               daysSinceStart % 7 == 0 && 
                               today.DayOfWeek == task.StartDate.DayOfWeek;
                    }
                    else
                    {
                        // Recur on specific day (1 = Monday, 7 = Sunday)
                        var selectedDay = int.Parse(preset.DaytoGenerate);
                        return (int)today.DayOfWeek == selectedDay;
                    }

                case 3: // Bi-Week
                    // Always recur on same day as start date, every two weeks
                    var weeksSinceStart = (today - task.StartDate.Date).Days / 7;
                    return weeksSinceStart > 0 && 
                           weeksSinceStart % 2 == 0 && 
                           today.DayOfWeek == task.StartDate.DayOfWeek;

                case 4: // Month
                    if (preset.DaytoGenerate == "0") 
                    {
                        // For all months, check if today is the same date as start date
                        // or last day of month if the date doesn't exist
                        var targetDay = task.StartDate.Day;
                        var lastDayOfMonth = DateTime.DaysInMonth(today.Year, today.Month);
                        var targetDate = Math.Min(targetDay, lastDayOfMonth);
                        return today.Day == targetDate;
                    }

                    var selectedMonths = preset.DaytoGenerate.Split(',')
                                      .Select(int.Parse)
                                      .ToList();
                    if (!selectedMonths.Contains(today.Month)) return false;

                    // Check if today is the target date or last day if target date doesn't exist
                    var originalDay = task.StartDate.Day;
                    var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
                    return today.Day == Math.Min(originalDay, daysInMonth);

                case 5: // Bi-Month
                    // First check if we're on the correct date or last possible date of month
                    var targetDayOfMonth = task.StartDate.Day;
                    var lastDayOfCurrentMonth = DateTime.DaysInMonth(today.Year, today.Month);
                    var targetDayForCurrentMonth = Math.Min(targetDayOfMonth, lastDayOfCurrentMonth);
                    
                    if (today.Day != targetDayForCurrentMonth) return false;

                    // Then check if this is the correct month (every other month from start)
                    var monthsSinceStart = ((today.Year - task.StartDate.Year) * 12) + today.Month - task.StartDate.Month;
                    return monthsSinceStart > 0 && monthsSinceStart % 2 == 0;

                default:
                    return false;
            }
        }

        private bool ShouldGenerateTaskTodayDefaultVersion(L1Tasks task, DateTime today)
        {
            switch (task.DefaultRecurringOptions)
            {
                case 1: // Day
                        return true; // All days

                case 2: // Week
                        // Recur on the same day as start date
                        var daysSinceStart = (today - task.StartDate.Date).Days;
                        return daysSinceStart > 0 &&
                               daysSinceStart % 7 == 0 &&
                               today.DayOfWeek == task.StartDate.DayOfWeek;
                case 3: // Month
                        // For all months, check if today is the same date as start date
                        // or last day of month if the date doesn't exist
                        var targetDay = task.StartDate.Day;
                        var lastDayOfMonth = DateTime.DaysInMonth(today.Year, today.Month);
                        var targetDate = Math.Min(targetDay, lastDayOfMonth);
                        return today.Day == targetDate;
                default:
                    return false;
            }
        }

        private async Task CreateNewTaskFromTemplate(L1Tasks task)
        {
            // Calculate the original time gap
            int daysDifference = (task.DueDate - task.StartDate).Days;

            var newTask = new L1Tasks
            {
                // Copy all relevant properties from template
                Title = task.Title,
                Description = task.Description,
                Category = task.Category,
                Priority = task.Priority,
                Status = (int)ItemTaskStatus.NotStarted,
                
                // Set new dates maintaining the original gap
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(daysDifference),
                
                // Important: Don't copy recurring settings
                IsRecurring = false,           // Set to false for the new task
                L1RecurringPresetId = null,      // Clear the preset reference
                DefaultRecurringOptions = null,
                GeneratedCount = null,         // Clear the generated count
                IsParentRecurring = false,
                
                // ... other properties
                IsNotification = task.IsNotification,
                L1NotificationPresetId = task.L1NotificationPresetId,
                DefaultNotificationOptions = task.DefaultNotificationOptions,

                CreatedByStudentId = task.CreatedByStudentId,
                LastModifiedDateTime = DateTime.Now,
            };

            dbContext.L1Tasks.Add(newTask);
            await dbContext.SaveChangesAsync(); // Save to get the new task ID

            // Copy subtasks if any
            if (task.L1SubTasks != null && task.L1SubTasks.Any())
            {
                foreach (var templateSubtask in task.L1SubTasks)
                {
                    // Calculate subtask gap
                    int subtaskGap = (task.DueDate - task.StartDate).Days;


                    var newSubtask = new L1SubTasks
                    {
                        L1TaskId = newTask.Id,
                        Title = templateSubtask.Title,
                        Category = templateSubtask.Category,
                        Priority = templateSubtask.Priority,
                        Status = (int)ItemTaskStatus.NotStarted,
                        StartDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(subtaskGap),
                        CreatedByStudentId = task.CreatedByStudentId,
                        LastModifiedDateTime = DateTime.Now,

                        // ... other properties
                        IsNotification = templateSubtask.IsNotification,
                        L1NotificationPresetId = templateSubtask.L1NotificationPresetId,
                        DefaultNotificationOptions = templateSubtask.DefaultNotificationOptions,
                    };

                    dbContext.L1SubTasks.Add(newSubtask);
                }
            }
        }
        #endregion


        //[HttpPost("api/utilities/refresh-queue")]
        [HttpGet]
        [Route("api/utilities/refresh-queue")]
        public async Task<IActionResult> RefreshNotificationQueue()
        {
            try
            {
                await _notificationManager.RefreshNotificationQueueAsync();
                return Ok(new { message = "Notification queue refreshed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing notification queue");
                return StatusCode(500, new { message = "Error refreshing notification queue" });
            }
        }

        //[HttpPost("api/utilities/process-now")]
        [HttpGet]
        [Route("api/utilities/process-now")]
        public async Task<IActionResult> ProcessNotificationsNow()
        {
            try
            {
                await _notificationManager.ProcessNotificationsAsync();
                return Ok(new { message = "Notifications processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notifications");
                return StatusCode(500, new { message = "Error processing notifications" });
            }
        }

        [HttpPost("api/utilities/test-send")]
        public async Task<IActionResult> TestNotification()
        {
            try
            {
                var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var message = new NotificationMessage
                {
                    Title = "Test Notification",
                    Message = "If you see this, notifications are working!",
                    Icon = "/path/to/icon.png"
                };

                await SendChromeNotificationAsync(studentId, message);
                return Ok(new { message = "Test notification sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test notification");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private async Task SendChromeNotificationAsync(
    string studentId,
    NotificationMessage message)
        {
            try
            {
                // Get user's subscription from database
                var pushSubscription = await dbContext.PushSubscriptions
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (pushSubscription == null)
                {
                    _logger.LogWarning($"No push subscription found for user {studentId}");
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
                    JsonSerializer.Serialize(notification),
                    vapidDetails
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending push notification to user {studentId}");
                throw;
            }
        }

        [HttpPost("api/utilities/subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionDto dto)
        {
            try
            {
                var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Check if subscription already exists
                var existingSubscription = await dbContext.PushSubscriptions
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (existingSubscription != null)
                {
                    // Update existing subscription
                    existingSubscription.Endpoint = dto.Endpoint;
                    existingSubscription.P256dh = dto.P256dh;
                    existingSubscription.Auth = dto.Auth;
                }
                else
                {
                    // Create new subscription
                    var subscription = new PushSubscriptions
                    {
                        StudentId = studentId,
                        Endpoint = dto.Endpoint,
                        P256dh = dto.P256dh,
                        Auth = dto.Auth,
                        CreatedAt = DateTime.UtcNow
                    };
                    await dbContext.PushSubscriptions.AddAsync(subscription);
                }

                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Subscription saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving push subscription");
                return StatusCode(500, new { message = "Error saving subscription" });
            }
        }

        [HttpPost("api/utilities/unsubscribe")]
        public async Task<IActionResult> Unsubscribe()
        {
            try
            {
                var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Check if subscription already exists
                var existingSubscription = await dbContext.PushSubscriptions
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (existingSubscription != null) {
                    dbContext.PushSubscriptions.Remove(existingSubscription);
                    await dbContext.SaveChangesAsync();
                    return Ok(new { message = "Unsubscription saved successfully" });
                }
                return Ok(new { message = "Unsubscription saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing push subscription");
                return StatusCode(500, new { message = "Error removing subscription" });
            }
        }

        public class PushSubscriptionDto
        {
            public string Endpoint { get; set; }
            public string P256dh { get; set; }
            public string Auth { get; set; }
        }

    }
}

