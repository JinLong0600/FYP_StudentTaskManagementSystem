using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1DiscussionForumLikes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int L1DiscussionForumId { get; set; }

        [Required]
        public string CreatedByStudentId { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; }

        // Navigation properties
        [ForeignKey("L1DiscussionForumId")]
        public virtual L1DiscussionForums L1DiscussionForums { get; set; }


        [ForeignKey("CreatedByStudentId")]
        public virtual L1Students L1Students { get; set; }
    }
}
