using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1SubTasks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int L1TaskId { get; set; }

        [Required]
        public string Title { get; set; }

        public int Category { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public bool IsNotification { get; set; }

        public int? L1NotificationPresetId { get; set; }

        public int? DefaultNotificationOptions { get; set; }

        [Required]
        public string CreatedByStudentId { get; set; }

        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        [Required]
        public DateTime DeletionDateTime { get; set; }

        [ForeignKey("L1TaskId")]
        public virtual L1Tasks L1Tasks { get; set; }

    }

}

