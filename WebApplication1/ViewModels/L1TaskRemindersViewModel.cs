using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1TaskRemindersViewModel
    {
        public int Id { get; set; }
        public int L1TaskId { get; set; }
        public DateTime ReminderDateTime { get; set; }
        public bool IsTriggered { get; set; }
    }



}
