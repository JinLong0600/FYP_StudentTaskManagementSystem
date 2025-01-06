using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1DiscussionComments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int L1DiscussionId { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Context { get; set; }
        
        [Required]
        public int CreatedByStudentId { get; set; }

        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        [Required]
        public DateTime DeletionDateTime { get; set; }

        [Required]
        public bool IsTakenDown { get; set; }

        [Required(AllowEmptyStrings = true)]
        public int L0AdminId { get; set; }

    }
}

