using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.Models
{
    public class L1RecurringTaskCounters
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int L1TaskId { get; set; }

        [Required]
        public int L1RecurringTaskSettingId { get; set; }

    }



}
