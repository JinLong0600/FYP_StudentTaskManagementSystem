﻿namespace StudentTaskManagement.ViewModels
{
    public class ForumDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string AuthorProfileImage { get; set; }
        public int Status { get; set; }
        public int LikeCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Tags { get; set; }
        public bool IsAuthor { get; set; }
        public bool IsLiked { get; set; }

    }
}
