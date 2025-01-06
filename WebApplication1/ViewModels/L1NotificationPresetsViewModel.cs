using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1NotificationPresetsViewModel
    {
        public int? Id { get; set; }


        [Required(ErrorMessage = "Preset name is required.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public int Type { get; set; }

        // Day type settings
        public int? ReminderDaysBefore { get; set; }


        // Minute type settings
        public int? ReminderHoursBefore { get; set; }

        public int? ReminderMinutesBefore { get; set; }

        public DateTime? ReminderTime { get; set; }

        public bool IsDaily { get; set; }
        public int CreatedByStudentId { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public DateTime DeletionDateTime { get; set; }
    }

}
