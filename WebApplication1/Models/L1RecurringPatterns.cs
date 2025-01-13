using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1RecurringPatterns
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Type { get; set; } /*Day = 1, Week, 2, Bi-Week = 3, Month = 4, Bi-Month = 5*/

        public string? DaytoGenerate { get; set; } /* when the type is "Day" 1-7 = Monday to Sunday and 0 = All Day, and for month is just the same which is 1-12 = jan to dec and 0 = all months*/

        public int? RecurringCount { get; set; } /* this is used for some user that they only want the task to be repeat at a certain number of time only*/

        public int Status { get; set; } //active //removed

        [DefaultValue(false)]
        public bool IsSystemDefault { get; set; }

        [Required]
        public string CreatedByStudentId { get; set; }

        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        [Required]
        public DateTime DeletionDateTime { get; set; }

        // Add inverse navigation property
        public virtual ICollection<L1Tasks> L1Tasks { get; set; }

        // Add inverse navigation property
        public virtual ICollection<L1SubTasks> L1SubTasks { get; set; }
    }




}
