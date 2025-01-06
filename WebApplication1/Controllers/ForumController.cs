using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentTaskManagement.Models;
using StudentTaskManagement.ViewModels;
using StudentTaskManagement.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using static StudentTaskManagement.Utilities.GeneralEnum;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace StudentTaskManagement.Controllers
{
    public class ForumController : Controller
    {
        private readonly StudentTaskManagementContext dbContext;
        private readonly ILogger<NotificationPresetController> _logger;
        private readonly UserManager<L1Students> _userManager;

        public ForumController(StudentTaskManagementContext dbContext,
            ILogger<NotificationPresetController> logger,
            UserManager<L1Students> userManager)
        {
            this.dbContext = dbContext;
            this._logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewData["ActiveMenu"] = "Forums";

            // Hardcoded sample data
            var categories = new List<L1DiscussionsViewModel>
            {
                new L1DiscussionsViewModel
                {
                    ForumId = 1,
                    Title = "General Discussion",
                    Description = "Talk about anything and everything",
                    CommentCount = 125,
                    Tags = new List<string> { "General", "Discussion", "Help" },
                    CreatedByStudentId = 1,
                    CreatedDateTime = DateTime.Now.AddHours(-12)
                },
                new L1DiscussionsViewModel
                {
                    ForumId = 2,
                    Title = "Technical Support",
                    Description = "Get help with technical issues",
                    CommentCount = 125,
                    Tags = new List<string> { "General", "Discussion", "Help" },
                    CreatedByStudentId = 1,
                    CreatedDateTime = DateTime.Now.AddHours(-12)
                },
                new L1DiscussionsViewModel
                {
                    ForumId = 3,
                    Title = "Announcements",
                    Description = "Important updates and news",
                    Tags = new List<string> { "General", "Discussion", "Help" },
                    CommentCount = 125,
                    CreatedByStudentId = 1,
                    CreatedDateTime = DateTime.Now.AddHours(-12)
                }
            };

            return View(categories);
        }
        
/*        [HttpGet]
        public IActionResult GetForumPosts(int page = 1, int pageSize = 10, string searchTerm = "", string sortOrder = "newest")
        {
            // Get your data from the database
            var query = GetForumCategories(); // Your data access method

            // Apply search if term provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => 
                    c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                    c.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Apply sorting
            query = sortOrder switch
            {
                "newest" => query.OrderByDescending(c => c.LatestPost?.CreatedAt).ToList(),
                "oldest" => query.OrderBy(c => c.LatestPost?.CreatedAt).ToList(),
                "mostActive" => query.OrderByDescending(c => c.CommentCount).ToList(),
                _ => query.OrderByDescending(c => c.LatestPost?.CreatedAt).ToList()
            };

            // Calculate pagination
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Generate HTML for the posts
            var html = RenderViewToString("_ForumPosts", items);

            return Json(new { 
                html = html, 
                totalPages = totalPages,
                currentPage = page
            });
        }

        private string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using var sw = new StringWriter();
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                sw,
                new HtmlHelperOptions()
            );

            viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
            return sw.GetStringBuilder().ToString();
        }*/


        /*[HttpGet]
        public IActionResult Details(int id)
        {
            var post = GetForumPost(id); // Your data access method
            var viewModel = new ForumDetailsViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorName = post.Author.Name,
                CreatedAt = post.CreatedAt,
                Tags = post.Tags,
                IsAuthor = post.AuthorId == User.GetUserId() // Check if current user is author
            };
            return View(viewModel);
        }
*/

        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = _userManager.GetUserId(User);

            var forum = await dbContext.L1DiscussionForums
                .Include(f => f.L1DiscussionForumComments)
                .FirstOrDefaultAsync(f => f.Id == id && f.DeletionDateTime == null);

            if (forum == null)
            {
                return NotFound();
            }

            var viewModel = new ForumDetailsViewModel
            {
                Id = forum.Id,
                Title = forum.Title,
                Content = forum.Description,
                CreatedAt = forum.CreatedDateTime,
                AuthorName = await GetUserName(forum.CreatedByStudentId),
                IsAuthor = forum.CreatedByStudentId == currentUserId,
                Tags = forum.Label?.Split(',').Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>(),
                Status = forum.Status
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int topicId, int page = 1, int pageSize = 10)
        {
            try
            {
                // Get comments data
                var commentsQuery = await dbContext.L1DiscussionForumComments
                    .Where(c => c.L1DiscussionForumId == topicId && c.DeletionDateTime == null)
                    .OrderByDescending(c => c.LastModifiedDateTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new
                    {
                        Id = c.Id,
                        Content = c.Context,
                        CreatedById = c.CreatedByStudentId,
                        CreatedAt = c.LastModifiedDateTime
                    })
                    .ToListAsync();

                var totalComments = await dbContext.L1DiscussionForumComments
                    .Where(c => c.L1DiscussionForumId == topicId && c.DeletionDateTime == null)
                    .CountAsync();

                // Get all unique user IDs
                var userIds = commentsQuery.Select(c => c.CreatedById).Distinct().ToList();
                
                // Get all users in one batch
                var users = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.UserName);

                var currentUserId = _userManager.GetUserId(User);

                // Map the data using the users dictionary
                var comments = commentsQuery.Select(c => new
                {
                    id = c.Id,
                    content = c.Content,
                    authorName = users.ContainsKey(c.CreatedById) ? users[c.CreatedById] : "Unknown User",
                    createdAt = c.CreatedAt.ToString("MMM dd, yyyy HH:mm"),
                    isAuthor = c.CreatedById == currentUserId
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
        public async Task<IActionResult> AddComment(int topicId, string content)
        {
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
                    CreatedByStudentId = "1",//_userManager.GetUserId(User),
                    Status = (int)ForumStatus.Active,
                    LastModifiedDateTime = DateTime.Now
                };

                dbContext.L1DiscussionForumComments.Add(comment);
                await dbContext.SaveChangesAsync();

                return Json(new { success = true, message = "Your comment has been added to the post" });
            }
            catch (Exception ex)
            { 
                    _logger.LogError(ex, "Error updating profile");
                    return Json(new { success = false, message = "Failed to add your comment to the post" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            try
            {
                var forum = await dbContext.L1DiscussionForums
                    .FirstOrDefaultAsync(f => f.Id == id && f.DeletionDateTime == null);

                if (forum == null)
                {
                    return Json(new { success = false, message = "Forum topic not found." });
                }

                if (forum.CreatedByStudentId != _userManager.GetUserId(User))
                {
                    return Json(new { success = false, message = "Unauthorized to delete this topic." });
                }

                forum.DeletionDateTime = DateTime.Now;
                await dbContext.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return Json(new { success = false, message = "Failed to add your comment to the post" });
            }
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
        [HttpPost]
        public IActionResult DeleteForum(int id)
        {
            // Simply return success for UI testing
            return Json(new { success = true });
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

        [HttpGet]
        [Route("Forum/CreateForum/{forumId?}")]
        public async Task<IActionResult> CreateForum(int? forumId)
        {
            // Get the list for dropdown and store in ViewBag
            ViewData["ActiveMenu"] = "Forums";

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
                    CreatedByStudentId = "1",
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

    }
}
