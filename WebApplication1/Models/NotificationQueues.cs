using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class NotificationQueues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int? SubtaskId { get; set; }
        public string UserId { get; set; }
        public int NotificationPresetId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastAttempt { get; set; }
        public string GroupKey { get; set; }
        public int Priority { get; set; }
        public int NotificationType { get; set; }
        public bool IsDaily { get; set; }
        public DateTime TaskDueDate { get; set; }

        // Navigation properties
        public virtual L1Tasks Task { get; set; }
        public virtual L1SubTasks SubTask { get; set; }
        public virtual L1NotificationPresets NotificationPreset { get; set; }
    }

}
