using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.Models
{
    public class L1ReportDiscussionSubmissions
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public int L1DiscussionId { get; set; }

        [Required]
        public int L1DiscussionCommentId { get; set; }

        [Required]
        public int ReportType { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string ReportReason { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public int CreatedById { get; set; }

        [Required]
        public DateTime SubmissionDateTime { get; set; }

        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        [Required]
        public int LastModifiedByAdminId { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string AdminReply { get; set; }
    }

}
