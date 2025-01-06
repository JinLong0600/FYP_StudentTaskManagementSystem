using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1ReportDiscussionSubmissionsViewModel
    {
        public int Id { get; set; }
        public int L1DiscussionId { get; set; }
        public int L1DiscussionCommentId { get; set; }
        public int ReportType { get; set; }
        public string ReportReason { get; set; }
        public int Status { get; set; }
        public int CreatedById { get; set; }
        public DateTime SubmissionDateTime { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public int LastModifiedByAdminId { get; set; }
        public string AdminReply { get; set; }
    }

}
