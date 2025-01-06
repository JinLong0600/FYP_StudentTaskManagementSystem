using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int Type { get; set; }

        // Day type settings
        public int? ReminderDaysBefore { get; set; }

        // Minute type settings
        public int? ReminderHoursBefore { get; set; }

        public int? ReminderMinutesBefore { get; set; }

        
        public DateTime? ReminderTime { get; set; }

        public int Status { get; set; } //active //removed

        [Required]
        public bool IsDaily { get; set; }

        [Required]
        public int CreatedByStudentId { get; set; }

        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        public DateTime? DeletionDateTime { get; set; }

        public virtual ICollection<L1Tasks> Tasks { get; set; }
        public virtual ICollection<L1SubTasks> SubTasks { get; set; }
        public virtual L1Students User { get; set; }
    }



}
