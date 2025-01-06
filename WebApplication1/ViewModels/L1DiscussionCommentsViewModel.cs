using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1DiscussionCommentsViewModel
    {
        public int Id { get; set; }
        public int L1DiscussionId { get; set; }
        public string Context { get; set; }
        public int CreatedByStudentId { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public DateTime DeletionDateTime { get; set; }
        public bool IsTakenDown { get; set; }
        public int L0AdminId { get; set; }

    }
}

