using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1DiscussionForumComments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int L1DiscussionForumId { get; set; }

        [Required]
        public string Context { get; set; }

        [Required]
        public int Status { get; set; }
        public bool IsDiscussionForumDeleted { get; set; }

        [Required]
        public string CreatedByStudentId { get; set; }

        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        public DateTime? DeletionDateTime { get; set; }
        
        [ForeignKey("L1DiscussionForumId")]
        public virtual L1DiscussionForums L1DiscussionForums { get; set; }

        [ForeignKey("CreatedByStudentId")]
        public virtual L1Students L1Students { get; set; }

    }
}

