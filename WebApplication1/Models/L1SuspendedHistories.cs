using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1SuspendedHistories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        [Required]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = true)]
        public int Description { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int PriorityLevel { get; set; }

        [Required]
        public bool IsRecurringTask { get; set; }

        [Required]
        public bool IsParentTask { get; set; }

        [Required]
        public bool IsReminderNeeded { get; set; }

        [Required]
        public int L1ReminderSettingId { get; set; }

        [Required]
        public int CreatedByStudentId { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }

        [Required]
        public DateTime DeletionDateTime { get; set; }
    }

}


