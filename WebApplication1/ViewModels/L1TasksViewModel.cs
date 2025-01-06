using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1TasksViewModel : _BaseViewModel
    {
        public bool IsEdit { get; set; }
        public int TaskId { get; set; }
        public string Title { get; set; }
        public int Category { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime DueDate { get; set; }

        public bool IsRecurring { get; set; }
        public int? L1RecurringPresetId { get; set; }
        public int? DefaultRecurringOptions { get; set; }

        public bool IsNotification { get; set; }
        public int? L1NotificationPresetId { get; set; }
        public int? DefaultNotificationOptions { get; set; }

        public int CreatedByStudentId { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public DateTime DeletionDateTime { get; set; }

        public string Subtasks { get; set; }
        public List<SubtaskListViewModel> SubtasksList { get; set; } = new List<SubtaskListViewModel>();

    }

    public class SubtaskListViewModel
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int Category { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime DueDate { get; set; }

        public bool IsNotification { get; set; }
        public int? L1NotificationPresetId { get; set; }
        public int? DefaultNotificationOptions { get; set; }

        public int CreatedByStudentId { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public DateTime DeletionDateTime { get; set; }
    }

}
