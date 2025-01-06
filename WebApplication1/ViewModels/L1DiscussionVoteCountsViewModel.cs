using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1DiscussionVoteCountsViewModel
    {
        public int Id { get; set; }
        public int L1StudentId { get; set; }
        public int VoteType { get; set; }

    }
}

