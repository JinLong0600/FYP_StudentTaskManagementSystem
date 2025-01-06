using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1RecurringTaskCountersViewModel
    {
        public int Id { get; set; }
        public int L1TaskId { get; set; }
        public int L1RecurringTaskSettingId { get; set; }
        public int Counter { get; set; }
    }



}
