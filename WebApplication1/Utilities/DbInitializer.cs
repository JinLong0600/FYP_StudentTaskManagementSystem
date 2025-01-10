using StudentTaskManagement.Models;
using static StudentTaskManagement.Utilities.GeneralEnum;

public static class DbInitializer
{
    public static void Initialize(StudentTaskManagementContext context)
    {
        // Check if system default already exists
        var test = context.L1NotificationPresets.Where(x => x.IsSystemDefault).FirstOrDefault();
        if (test == null)
        {
            var systemDefaultNotification = new L1NotificationPresets
            {
                Id = 0,
                Name = "- Default Notification Preset -",
                Description = "Default notification settings",
                Status = (int)PresetPatternStatus.Active,
                IsSystemDefault = true,
                IsDaily = false,
                CreatedByStudentId = "System",
                LastModifiedDateTime = DateTime.Now,
            };

            context.L1NotificationPresets.Add(systemDefaultNotification);
            context.SaveChanges();
        }

        // Check if system default already exists
        if (context.L1RecurringPresets.Where(x => x.IsSystemDefault).FirstOrDefault() == null)
        {
            var systemDefaultRecurring = new L1RecurringPatterns
            {
                Id = 0,
                Name = "- Default Recurring Preset -",
                Description = "Default recurring settings",
                Type = 0,
                Status = (int)PresetPatternStatus.Active,
                IsSystemDefault = true,
                CreatedByStudentId = "System",
                LastModifiedDateTime = DateTime.Now,
            };

            context.L1RecurringPresets.Add(systemDefaultRecurring);
            context.SaveChanges();
        }
    }
}