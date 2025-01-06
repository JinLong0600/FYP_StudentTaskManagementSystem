using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1DiscussionsViewModel
    {
        public bool IsEdit { get; set; }
        public int ForumId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CommentCount { get; set; }
        public int Category { get; set; }
        public List<string> Tags { get; set; }
        public string LabelTags { get; set; }
        public int Status { get; set; }
        public int CreatedByStudentId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime DeletionDateTime { get; set; }
    }

}
