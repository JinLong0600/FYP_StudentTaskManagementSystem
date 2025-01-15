using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentTaskManagement.Models;
using StudentTaskManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using static StudentTaskManagement.Utilities.GeneralEnum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace StudentTaskManagement.Controllers
{
    [Authorize]
    public class ForumController : _BaseController
    {
        protected readonly StudentTaskManagementContext dbContext;
        protected readonly ILogger _logger;
        private readonly UserManager<L1Students> _userManager;
        private readonly SignInManager<L1Students> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ForumController(StudentTaskManagementContext dbContext, ILogger<NotificationPresetController> logger, UserManager<L1Students> userManager, SignInManager<L1Students> signInManager, IEmailService emailService, IWebHostEnvironment webHostEnvironment)
        : base(dbContext, logger, userManager, signInManager, emailService, webHostEnvironment)
        {
            this.dbContext = dbContext;
            this._logger = logger;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailService = emailService;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            ViewData["ActiveMenu"] = "DiscussionForum";
            return View();
        }

        public IActionResult MyPost()
        {
            ViewData["ActiveMenu"] = "MyPost";
            return View();
        }

        // View Forum Details Page
        public async Task<IActionResult> Details(int id)
        {
            ViewData["ActiveMenu"] = "MyPost";
            var currentUserId = _userManager.GetUserId(User);

            var forum = await dbContext.L1DiscussionForums
                .Include(f => f.L1DiscussionForumComments)
                .Include(f => f.L1Students)
                .FirstOrDefaultAsync(f => f.Id == id && f.DeletionDateTime == null);

            if (forum == null)
            {
                return NotFound();
            }

            // Check if the current user has liked this post
            var isLiked = await dbContext.L1DiscussionForumLikes
                .AnyAsync(l => l.L1DiscussionForumId == id && l.CreatedByStudentId == currentUserId);

            var viewModel = new ForumDetailsViewModel
            {
                Id = forum.Id,
                Title = forum.Title,
                Content = forum.Description,
                CreatedAt = forum.CreatedDateTime,
                LikeCount = forum.LikeCount,
                AuthorName = await GetUserName(forum.CreatedByStudentId),
                AuthorProfileImage = forum.L1Students.ProfileImage,
                IsAuthor = forum.CreatedByStudentId == currentUserId,
                IsLiked = isLiked,
                Tags = forum.Label?.Split(',').Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>(),
                Status = forum.Status
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AjaxGetComments(int topicId, int page = 1, int pageSize = 10)
        {
            try
           {
                // Get comments data
                var commentsQuery = await dbContext.L1DiscussionForumComments
                    .Include(c => c.L1Students)
                    .Where(c => c.L1DiscussionForumId == topicId && c.Status == (int)ForumStatus.Active)
                    .OrderByDescending(c => c.LastModifiedDateTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new
                    {
                        Id = c.Id,
                        Content = c.Context,
                        CreatedById = c.CreatedByStudentId,
                        CreatedByUsername = c.L1Students.UserName,
                        StudentIdProfileImage = c.L1Students.ProfileImage,
                        CreatedAt = c.LastModifiedDateTime,
                    })
                    .ToListAsync();

                var totalComments = await dbContext.L1DiscussionForumComments
                    .Where(c => c.L1DiscussionForumId == topicId && c.DeletionDateTime == null)
                    .CountAsync();

                // Get all unique user IDs
                /*var userIds = commentsQuery.Select(c => c.CreatedById).Distinct().ToList();*/
                
                // Get all users in one batch
/*                var users = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.UserName);*/

                // Map the data using the users dictionary
                var comments = commentsQuery.Select(c => new
                {
                    id = c.Id,
                    content = c.Content,
                    authorName = c.CreatedByUsername, //users.ContainsKey(c.CreatedById) ? users[c.CreatedById] : "Unknown User",
                    authorProfileImage = c.StudentIdProfileImage,
                    createdAt = c.CreatedAt.ToString("MMM dd, yyyy HH:mm"),
                    isAuthor = c.CreatedById == LoginStudentId
                }).ToList();

                var hasMore = (page * pageSize) < totalComments;

                return Json(new { comments, hasMore });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments");
                return Json(new { success = false, message = "Failed to load comments" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjaxAddComment(int topicId, string content)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    return Json(new { success = false, message = "Comment content is required." });
                }

                var forum = await dbContext.L1DiscussionForums
                    .FirstOrDefaultAsync(f => f.Id == topicId && f.DeletionDateTime == null);

                if (forum == null)
                {
                    return Json(new { success = false, message = "Forum topic not found." });
                }

                var comment = new L1DiscussionForumComments
                {
                    L1DiscussionForumId = topicId,
                    Context = content,
                    CreatedByStudentId = LoginStudentId,
                    Status = (int)ForumStatus.Active,
                    IsDiscussionForumDeleted = false,
                    LastModifiedDateTime = DateTime.Now
                };

                dbContext.L1DiscussionForumComments.Add(comment);
                
                // Increment the comment count
                forum.CommentCount++;
                forum.LastModifiedDate = DateTime.Now; // Optional: update last modified date

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { 
                    success = true, 
                    message = "Your comment has been added to the post",
                    commentCount = forum.CommentCount // Return updated count
                });
            }
            catch (Exception ex)
            { 
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating profile");
                return Json(new { success = false, message = "Failed to add your comment to the post" });
            }
        }


        // Create & Edit Forum Page
        [HttpGet]
        [Route("Forum/CreateForum/{forumId?}")] 
        public async Task<IActionResult> CreateForum(int? forumId)
        {
            // Get the list for dropdown and store in ViewBag
            ViewData["ActiveMenu"] = "MyPost";

            if (forumId != null)
            {
                var forum = await dbContext.L1DiscussionForums.FirstOrDefaultAsync(t => t.Id == forumId);

                if (forum == null)
                {
                    return NotFound();
                }

                var viewModel = new L1DiscussionsViewModel
                {
                    IsEdit = true,
                    // Map main task properties
                    ForumId = forumId.Value,
                    Title = forum.Title,
                    Category = forum.Category,
                    Description = forum.Description,
                    LabelTags = forum.Label,
                    Status = forum.Status,
                };

                return View(viewModel);
            }
            else
            {
                var model = new L1DiscussionsViewModel();
                model.IsEdit = false;
                return View(model);
            }
        }

        // Create Function
        [HttpPost]
        public async Task<IActionResult> Add(L1DiscussionsViewModel viewModel)
        {
            try
            {
                var forum = new L1DiscussionForums
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Category = viewModel.Category,
                    Label = viewModel.LabelTags,
                    Status = (int)ForumStatus.Active,
                    LikeCount = 0,
                    CommentCount = 0,
                    CreatedByStudentId = LoginStudentId,
                    CreatedDateTime = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                };
                await dbContext.L1DiscussionForums.AddAsync(forum);
                await dbContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Forum posted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on posting forum");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while posting the forum. Please try again."
                });
            }
        }

        // Edit Function
        [HttpPost]
        [Route("Forum/Update/{forumId?}")]
        public async Task<IActionResult> Update(int forumId, L1DiscussionsViewModel viewModel)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // Get existing task with its subtasks
                var existingForum = await dbContext.L1DiscussionForums
                    .Include(t => t.L1DiscussionForumComments)
                    .FirstOrDefaultAsync(t => t.Id == forumId);

                if (existingForum == null)
                {
                    return Json(new { success = false, message = "Forum not found" });
                }

                // Update main task properties
                existingForum.Title = viewModel.Title;
                existingForum.Category = viewModel.Category;
                existingForum.Description = viewModel.Description;
                existingForum.Label = viewModel.LabelTags;
                existingForum.Status = viewModel.Status;
                existingForum.LastModifiedDate = DateTime.Now;

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Forum updated successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating forum");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating forum. Please try again."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Forum/DeleteForum/{id?}")]
        public async Task<IActionResult> DeleteForum(int id)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var forum = await dbContext.L1DiscussionForums
                    .Include(f => f.L1DiscussionForumComments)
                    .Include(f => f.L1DiscussionForumLikes)
                    .FirstOrDefaultAsync(f => f.Id == id && f.Status != (int)ForumStatus.Deleted);

                if (forum == null)
                {
                    return Json(new { success = false, message = "Forum not found." });
                }

                // Check if the current user is the author
                if (forum.CreatedByStudentId != LoginStudentId)
                {
                    return Json(new { success = false, message = "You are not authorized to delete this forum." });
                }

                // Soft delete the forum
                forum.DeletionDateTime = DateTime.Now;
                forum.Status = (int)ForumStatus.Deleted;
                forum.CreatedByStudentId = LoginStudentId;

                // Soft delete all comments
                if (forum.L1DiscussionForumComments?.Any() == true)
                {
                    foreach (var comment in forum.L1DiscussionForumComments)
                    {
                        comment.IsDiscussionForumDeleted = true;
                        comment.DeletionDateTime = DateTime.Now;
                        comment.LastModifiedDateTime = DateTime.Now;
                    }
                }
                
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Forum has been deleted successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting forum");
                return Json(new { success = false, message = "An error occurred while deleting the forum." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjaxToggleLike(int topicId)
        {
            try
            {
                // Get the forum topic first
                var forumTopic = await dbContext.L1DiscussionForums
                    .FirstOrDefaultAsync(f => f.Id == topicId);

                if (forumTopic == null)
                {
                    return Json(new { success = false, message = "Topic not found" });
                }

                var existingLike = await dbContext.L1DiscussionForumLikes
                    .FirstOrDefaultAsync(l => l.L1DiscussionForumId == topicId && l.CreatedByStudentId == LoginStudentId);

                if (existingLike != null)
                {
                    // Unlike
                    dbContext.L1DiscussionForumLikes.Remove(existingLike);
                    // Decrease like count
                    forumTopic.LikeCount = Math.Max(0, forumTopic.LikeCount - 1); // Ensure it doesn't go below 0
                    await dbContext.SaveChangesAsync();
                    return Json(new { 
                        success = true, 
                        isLiked = false,
                        likeCount = forumTopic.LikeCount 
                    });
                }
                else
                {
                    // Like
                    var newLike = new L1DiscussionForumLikes
                    {
                        L1DiscussionForumId = topicId,
                        CreatedByStudentId = LoginStudentId,
                        CreatedDateTime = DateTime.Now
                    };
                    await dbContext.L1DiscussionForumLikes.AddAsync(newLike);
                    // Increase like count
                    forumTopic.LikeCount++;
                    await dbContext.SaveChangesAsync();
                    return Json(new { 
                        success = true, 
                        isLiked = true,
                        likeCount = forumTopic.LikeCount 
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update like status" });
            }
        }

        public async Task<IActionResult> AjaxGetForumPosts(string titleSearch, string tags,
            string status, string sortOrder, int page = 1, int pageSize = 10)
        {
            try 
            {
                var query = dbContext.L1DiscussionForums
                    .Include(f => f.L1Students)
                    .Where(f => f.Status != (int)ForumStatus.Deleted);

                // Apply filters
                if (!string.IsNullOrEmpty(titleSearch))
                    query = query.Where(f => f.Title.Contains(titleSearch));

/*                if (tags != null && tags.Length > 0)
                    query = query.Where(f => tags.All(t => f.Label.Contains(t)));*/

                if (!string.IsNullOrEmpty(tags))
                {
                    var searchTags = tags.Split(',')
                        .Select(t => t.Trim().ToLower())
                        .ToList();

                    query = query.Where(post =>
                        post.Label != null &&
                        searchTags.Any(searchTag =>
                            post.Label.ToLower().Contains(searchTag)
                        )
                    );
                }

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(f => f.Status == Convert.ToInt32(status));

                // Apply sorting
                query = sortOrder switch
                {
                    "newest" => query.OrderByDescending(f => f.CreatedDateTime),
                    "oldest" => query.OrderBy(f => f.CreatedDateTime),
                    "mostLiked" => query.OrderByDescending(f => f.LikeCount),
                    "mostCommented" => query.OrderByDescending(f => f.CommentCount),
                    _ => query.OrderByDescending(f => f.CreatedDateTime) // "newest" is default
                };

                var totalPosts = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

                var posts = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        forumId = p.Id,
                        title = p.Title,
                        description = p.Description,
                        label = p.Label,
                        status = p.Status,
                        likeCount = p.LikeCount,
                        commentCount = p.CommentCount,
                        lastModifiedDate = GetTimeAgo(p.CreatedDateTime),
                        createdByStudentId = p.CreatedByStudentId,
                        authorName = p.L1Students.UserName,
                        authorProfileImage = p.L1Students.ProfileImage,
                        isNew = p.CreatedDateTime >= DateTime.Now.AddHours(-24),
                        isResolved = p.Status == (int)ForumStatus.Active ? false : true
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = posts,
                    totalPages = totalPages,
                    currentPage = page
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on posting forum");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while posting the forum. Please try again."
                });
            }
            
        }

        public async Task<IActionResult> AjaxGetMyPosts(string titleSearch, string tags,
            string status, string sortOrder, int page = 1, int pageSize = 10)
        {
            try
            {
                var query = dbContext.L1DiscussionForums
                .Include(f => f.L1Students)
                .Where(f => f.CreatedByStudentId == LoginStudentId && f.Status != (int)ForumStatus.Deleted);

                // Apply filters
                if (!string.IsNullOrEmpty(titleSearch))
                    query = query.Where(f => f.Title.Contains(titleSearch));

                /*                if (tags != null && tags.Length > 0)
                                    query = query.Where(f => tags.All(t => f.Label.Contains(t)));*/

                if (!string.IsNullOrEmpty(tags))
                {
                    var searchTags = tags.Split(',')
                        .Select(t => t.Trim().ToLower())
                        .ToList();

                    query = query.Where(post =>
                        post.Label != null &&
                        searchTags.Any(searchTag =>
                            post.Label.ToLower().Contains(searchTag)
                        )
                    );
                }

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(f => f.Status == Convert.ToInt32(status));

                // Apply sorting
                query = sortOrder switch
                {
                    "newest" => query.OrderByDescending(f => f.CreatedDateTime),
                    "oldest" => query.OrderBy(f => f.CreatedDateTime),
                    "mostLiked" => query.OrderByDescending(f => f.LikeCount),
                    "mostCommented" => query.OrderByDescending(f => f.CommentCount),
                    _ => query.OrderByDescending(f => f.CreatedDateTime) // "newest" is default
                };

                var totalPosts = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

                var posts = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        forumId = p.Id,
                        title = p.Title,
                        description = p.Description,
                        label = p.Label,
                        status = p.Status,
                        likeCount = p.LikeCount,
                        commentCount = p.CommentCount,
                        lastModifiedDate = p.CreatedDateTime.ToString("dd-MM-yyyy, hh:mm tt"),
                        createdByStudentId = p.CreatedByStudentId,
                        authorName = p.L1Students.UserName,
                        isNew = p.CreatedDateTime >= DateTime.Now.AddHours(-24),
                        isResolved = p.Status == (int)ForumStatus.Active ? false : true
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = posts,
                    totalPages = totalPages,
                    currentPage = page
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on posting forum");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while posting the forum. Please try again."
                });
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjaxDeleteComment(int commentId)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var comment = await dbContext.L1DiscussionForumComments
                    .FirstOrDefaultAsync(c => c.Id == commentId && c.Status == (int)ForumStatus.Active);

                if (comment == null)
                {
                    return Json(new { success = false, message = "Comment not found." });
                }

                // Check if user is authorized to delete this comment
                if (comment.CreatedByStudentId != LoginStudentId)
                {
                    return Json(new { success = false, message = "You are not authorized to delete this comment." });
                }

                // Get the forum to update comment count
                var forum = await dbContext.L1DiscussionForums
                    .FirstOrDefaultAsync(f => f.Id == comment.L1DiscussionForumId);

                if (forum == null)
                {
                    return Json(new { success = false, message = "Forum topic not found." });
                }

                // Soft delete the comment
                comment.Status = (int)ForumStatus.Deleted;
                comment.DeletionDateTime = DateTime.Now;
                comment.CreatedByStudentId = LoginStudentId;
                
                // Decrease the comment count
                forum.CommentCount = Math.Max(0, forum.CommentCount - 1); // Ensure it doesn't go below 0
                forum.LastModifiedDate = DateTime.Now;

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { 
                    success = true, 
                    message = "Comment has been deleted",
                    commentCount = forum.CommentCount
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting comment");
                return Json(new { success = false, message = "Failed to delete the comment" });
            }
        }

        #region Function

        public static string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;

            if (span.TotalMinutes < 2) return "just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            return dateTime.ToString("dd-MM-yyyy, hh:mm tt");
        }

        private async Task<string> GetUserName(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName ?? "Unknown User";
        }

        private static async Task<string> GetUserNameStatic(string userId, UserManager<L1Students> userManager)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user?.UserName ?? "Unknown User";
        }

        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            // Add default option
            selectListItems.Add(new SelectListItem { Text = "- Please select -", Value = "" });
            selectListItems.Add(new SelectListItem { Text = "General Discussions", Value = ((int)ForumCategory.GeneralDiscussions).ToString() });
            selectListItems.Add(new SelectListItem { Text = "HomeworkHelp", Value = ((int)ForumCategory.HomeworkHelp).ToString() });
            selectListItems.Add(new SelectListItem { Text = "Exam Prep", Value = ((int)ForumCategory.ExamsTestPrep).ToString() });
            selectListItems.Add(new SelectListItem { Text = "Interships & Volunteering", Value = ((int)ForumCategory.InternshipsVolunteering).ToString() });
            selectListItems.Add(new SelectListItem { Text = "Scholarships & Financial Aid", Value = ((int)ForumCategory.ScholarshipsFinancialAid).ToString() });
            selectListItems.Add(new SelectListItem { Text = "Q & A", Value = ((int)ForumCategory.QnA).ToString() });
            selectListItems.Add(new SelectListItem { Text = "Study Groups", Value = ((int)ForumCategory.StudyGroups).ToString() });
            selectListItems.Add(new SelectListItem { Text = "Peer Advice", Value = ((int)ForumCategory.PeerAdvice).ToString() });


            return selectListItems.OrderBy(x => x.Text).ToList();
        }

        #endregion
    }



}
