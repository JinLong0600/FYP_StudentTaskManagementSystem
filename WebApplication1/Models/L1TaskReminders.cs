using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1TaskReminders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int L1TaskId { get; set; }

        [Required]
        public DateTime ReminderDateTime { get; set; }

        [Required]
        public bool IsTriggered { get; set; }
    }

}
