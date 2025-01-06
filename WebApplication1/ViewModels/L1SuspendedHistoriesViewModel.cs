using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1SuspendedHistoriesViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Description { get; set; }
        public int Status { get; set; }
        public DateTime DueDate { get; set; }
        public int PriorityLevel { get; set; }
        public bool IsRecurringTask { get; set; }
        public bool IsParentTask { get; set; }
        public bool IsReminderNeeded { get; set; }
        public int L1ReminderSettingId { get; set; }
        public int CreatedByStudentId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime DeletionDateTime { get; set; }
    }

}


