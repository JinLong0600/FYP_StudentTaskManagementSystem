using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1Tasks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int Category { get; set; }

        public string? Description { get; set; }

        [Required(AllowEmptyStrings = true)]
        public int Status { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public bool IsRecurring { get; set; }

        public bool? IsParentRecurring { get; set; } // false = child, else null && true = parent
        public int? L1RecurringPresetId { get; set; }

        public int? DefaultRecurringOptions { get; set; }

        public int? GeneratedCount { get; set; }


        [Required]
        public bool IsNotification { get; set; }

        public int? L1NotificationPresetId { get; set; }

        public int? DefaultNotificationOptions { get; set; }

        public string CreatedByStudentId { get; set; }

        [Required(AllowEmptyStrings = true)]
        public DateTime LastModifiedDateTime { get; set; }

        [Required(AllowEmptyStrings = true)]
        public DateTime DeletionDateTime { get; set; }

        public virtual ICollection<L1SubTasks> L1SubTasks { get; set; }

        [ForeignKey("L1RecurringPresetId")]
        public virtual L1RecurringPatterns L1RecurringPatterns { get; set; }  // Changed from L1RecurringPresets to L1RecurringPreset (singular)

        [ForeignKey("L1NotificationPresetId")]
        public virtual L1NotificationPresets L1NotificationPresets { get; set; }  // Changed from L1RecurringPresets to L1RecurringPreset (singular)


    }

}
