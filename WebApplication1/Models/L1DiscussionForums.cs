using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1DiscussionForums
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int Category { get; set; }

        public string Label { get; set; }

        [Required]
        public int Status { get; set; }

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }

        [Required]
        public string CreatedByStudentId { get; set; }
        [Required]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }
        
        public DateTime? DeletionDateTime { get; set; }

        public virtual ICollection<L1DiscussionForumComments> L1DiscussionForumComments { get; set; }

        public virtual ICollection<L1DiscussionForumLikes> L1DiscussionForumLikes { get; set; }

        [ForeignKey("CreatedByStudentId")]
        public virtual L1Students L1Students { get; set; }

    }
}
