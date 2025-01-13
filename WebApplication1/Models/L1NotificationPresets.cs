using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace StudentTaskManagement.Models
{
    public class L1NotificationPresets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public int Type { get; set; } // Type value: 1 is used for reminder at how many day before the due date, Type value: 2 is for reminder at how many hours or minutes before the due date

        // Day type settings
        public int? ReminderDaysBefore { get; set; } // will have value only when Type value is 2 else null

        // Minute type settings
        public int? ReminderHoursBefore { get; set; } // will have value only when Type value is 2 else null

        public int? ReminderMinutesBefore { get; set; } // will have value only when Type value is 1 else null

        public DateTime? ReminderTime { get; set; } // will have value only when Type value is 1 else null. example: reminder 2 day before, at (ReminderTime)

        [Required]
        public bool IsDaily { get; set; } // will have value only when Type value is 2 else null

        public int Status { get; set; } //active //removed

        [DefaultValue(false)]
        public bool IsSystemDefault { get; set; } // this one can ignore first

        [Required]
        public string CreatedByStudentId { get; set; }

        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        public DateTime? DeletionDateTime { get; set; }



        public virtual ICollection<L1Tasks> L1Tasks { get; set; }
        public virtual ICollection<L1SubTasks> L1SubTasks { get; set; }
        public virtual L1Students User { get; set; }

    }



}
