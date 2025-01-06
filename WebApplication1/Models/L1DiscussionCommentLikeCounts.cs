using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.Models
{
    public class L1DiscussionCommentLikeCounts
    {
        [Key]
        public int L1DiscussionCommentId { get; set; }

        [Required]
        public int L1StudentId { get; set; }

    }
}

