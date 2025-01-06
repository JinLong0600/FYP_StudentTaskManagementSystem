using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1Discussions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string Label { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public string InsitutionName { get; set; }

        [Required]
        public int PrivacySetting { get; set; }

        [Required]
        public int CreatedByStudentId { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }
        
        [Required]
        public DateTime DeletionDateTime { get; set; }

        [Required]
        public bool IsTakenDown { get; set; }

        [Required]
        public int L0AdminId { get; set; }
    }
}
