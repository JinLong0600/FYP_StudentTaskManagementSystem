using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.ViewModels
{
    public class L1RecurringPresetViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Type { get; set; } //days //week //bi-weekly //month //bi-monthly //quartely

        [Required]
        public string DaytoGenerate { get; set; } //choose day // choose start from which day (Mon, Tue, etc...)

        public string DaytoGenerateHidden { get; set; } //choose day // choose start from which day (Mon, Tue, etc...)
        
        public int? RecurringCount { get; set; } //week //bi-weekly //month //bi-monthly //quartely

        public int Status { get; set; } //active //removed

        //rescheduled features
        //time variantions

        public int CreatedByStudentId { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public DateTime? DeletionDateTime { get; set; }
    }



}
