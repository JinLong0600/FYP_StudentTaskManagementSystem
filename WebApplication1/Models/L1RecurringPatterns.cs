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
        public int Type { get; set; } //days //week //bi-weekly //month //bi-monthly //quartely

        public string DaytoGenerate { get; set; } //choose day // choose start from which day (Mon, Tue, etc...)

        public int? RecurringCount { get; set; } //number

        public int Status { get; set; } //active //removed


        //rescheduled features
        //time variantions

        [Required]
        public int CreatedByStudentId { get; set; }

        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        [Required]
        public DateTime DeletionDateTime { get; set; }
    }



}
