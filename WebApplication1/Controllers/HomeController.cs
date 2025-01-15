using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentTaskManagement.Controllers;
using StudentTaskManagement.Models;
using StudentTaskManagement.Security;
using StudentTaskManagement.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using WebApplication1.Models;
using static StudentTaskManagement.Utilities.GeneralEnum;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class HomeController : _BaseController
    {
        protected readonly StudentTaskManagementContext dbContext;
        protected readonly ILogger _logger;
        private readonly UserManager<L1Students> _userManager;
        private readonly SignInManager<L1Students> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(StudentTaskManagementContext dbContext, ILogger<NotificationPresetController> logger, UserManager<L1Students> userManager, SignInManager<L1Students> signInManager, IEmailService emailService, IWebHostEnvironment webHostEnvironment)
        : base(dbContext, logger, userManager, signInManager, emailService, webHostEnvironment)
        {
            this.dbContext = dbContext;
            this._logger = logger;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailService = emailService;
            this._webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["ActiveMenu"] = "Home";
            return View();
        }

        public IActionResult EmailContentConfrimation()
        {
            return View();
        }

        public IActionResult EmailContentResetPassword()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult TestNotification()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AjaxGetTaskCount(int period)
        {
            try
            {
                var today = DateTime.Today;
                IQueryable<L1Tasks> taskQuery = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId).AsQueryable();

                switch (period)
                {
                    case 1: // This week
                        var currentDayOfWeek = (int)today.DayOfWeek;
                        var mondayOffset = currentDayOfWeek == 0 ? -6 : 1 - currentDayOfWeek;
                        var currentWeekStart = today.AddDays(mondayOffset);
                        var currentWeekEnd = currentWeekStart.AddDays(6);
                        taskQuery = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId 
                            && t.DueDate >= currentWeekStart 
                            && t.DueDate <= currentWeekEnd);
                        break;

                    case 2: // Next week
                        var nextWeekStart = today.AddDays(7 - (int)today.DayOfWeek + 1);
                        var nextWeekEnd = nextWeekStart.AddDays(6);
                        taskQuery = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId 
                            && t.DueDate >= nextWeekStart 
                            && t.DueDate <= nextWeekEnd);
                        break;

                    case 3: // Last week
                        var lastWeekStart = today.AddDays(-7 - (int)today.DayOfWeek + 1);
                        var lastWeekEnd = lastWeekStart.AddDays(6);
                        taskQuery = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId 
                            && t.DueDate >= lastWeekStart 
                            && t.DueDate <= lastWeekEnd);
                        break;

                    case 4: // This month
                        var currentMonthStart = new DateTime(today.Year, today.Month, 1);
                        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
                        taskQuery = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId 
                            && t.DueDate >= currentMonthStart 
                            && t.DueDate <= currentMonthEnd);
                        break;

                    default:
                        return Json(new { success = false, message = "Invalid period selected" });
                }

                // Calculate statistics based on the selected period
                int total = taskQuery.Count();
                int completed = taskQuery.Count(t => t.Status == (int)ItemTaskStatus.Completed);
                int inProgress = taskQuery.Count(t => t.Status == (int)ItemTaskStatus.InProgress);
                int notStarted = taskQuery.Count(t => t.Status == (int)ItemTaskStatus.NotStarted);
                int onHold = taskQuery.Count(t => t.Status == (int)ItemTaskStatus.OnHold);
                int overdue = taskQuery.Count(t => t.Status == (int)ItemTaskStatus.Overdue);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        totalTask = total,
                        completedTask = completed,
                        inProgressTask = inProgress,
                        NotStartedTask = notStarted,
                        onHoldTask = onHold,
                        overdueTask = overdue,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tasks");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while fetching tasks"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AjaxGetTaskCountTable(int period)
        {
            try
            {
                var today = DateTime.Today;
                IQueryable<L1Tasks> taskQuery = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId).AsQueryable();

                switch (period)
                {
                    case 1: // This week
                        var currentDayOfWeek = (int)today.DayOfWeek;
                        var mondayOffset = currentDayOfWeek == 0 ? -6 : 1 - currentDayOfWeek;
                        var currentWeekStart = today.AddDays(mondayOffset);
                        var currentWeekEnd = currentWeekStart.AddDays(6);
                        taskQuery = taskQuery.Where(t => t.CreatedByStudentId == LoginStudentId && t.DueDate >= currentWeekStart && t.DueDate <= currentWeekEnd);
                        break;

                    case 2: // Next week
                        var nextWeekStart = today.AddDays(7 - (int)today.DayOfWeek + 1);
                        var nextWeekEnd = nextWeekStart.AddDays(6);
                        taskQuery = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId && t.DueDate >= nextWeekStart && t.DueDate <= nextWeekEnd);
                        break;

                    case 3: // Last week
                        var lastWeekStart = today.AddDays(-7 - (int)today.DayOfWeek + 1);
                        var lastWeekEnd = lastWeekStart.AddDays(6);
                        taskQuery = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId && t.DueDate >= lastWeekStart && t.DueDate <= lastWeekEnd);
                        break;

                    case 4: // This month
                        var currentMonthStart = new DateTime(today.Year, today.Month, 1);
                        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
                        taskQuery = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId && t.DueDate >= currentMonthStart && t.DueDate <= currentMonthEnd);
                        break;

                    default:
                        return Json(new { success = false, message = "Invalid period selected" });
                }
                
                var mon = taskQuery.ToList().Where(t => t.LastModifiedDateTime.DayOfWeek == DayOfWeek.Monday);
                int monCompleted = mon.Where(t => t.Status == (int)ItemTaskStatus.Completed).Count();
                int monInProgress = mon.Where(t => t.Status == (int)ItemTaskStatus.InProgress).Count();
                int monNotStarted = mon.Where(t => t.Status == (int)ItemTaskStatus.NotStarted).Count();
                int monOnHold = mon.Where(t => t.Status == (int)ItemTaskStatus.OnHold).Count();
                int monOverdue = mon.Where(t => t.Status == (int)ItemTaskStatus.Overdue).Count();

                var tue = taskQuery.ToList().Where(t => t.LastModifiedDateTime.DayOfWeek == DayOfWeek.Tuesday);
                int tueCompleted = tue.Where(t => t.Status == (int)ItemTaskStatus.Completed).Count();
                int tueInProgress = tue.Where(t => t.Status == (int)ItemTaskStatus.InProgress).Count();
                int tueNotStarted = tue.Where(t => t.Status == (int)ItemTaskStatus.NotStarted).Count();
                int tueOnHold = tue.Where(t => t.Status == (int)ItemTaskStatus.OnHold).Count();
                int tueOverdue = tue.Where(t => t.Status == (int)ItemTaskStatus.Overdue).Count();

                var wed = taskQuery.ToList().Where(t => t.LastModifiedDateTime.DayOfWeek == DayOfWeek.Wednesday);
                int wedCompleted = wed.Where(t => t.Status == (int)ItemTaskStatus.Completed).Count();
                int wedInProgress = wed.Where(t => t.Status == (int)ItemTaskStatus.InProgress).Count();
                int wedNotStarted = wed.Where(t => t.Status == (int)ItemTaskStatus.NotStarted).Count();
                int wedOnHold = wed.Where(t => t.Status == (int)ItemTaskStatus.OnHold).Count();
                int wedOverdue = wed.Where(t => t.Status == (int)ItemTaskStatus.Overdue).Count();

                var thu = taskQuery.ToList().Where(t => t.LastModifiedDateTime.DayOfWeek == DayOfWeek.Thursday);
                int thuCompleted = thu.Where(t => t.Status == (int)ItemTaskStatus.Completed).Count();
                int thuInProgress = thu.Where(t => t.Status == (int)ItemTaskStatus.InProgress).Count();
                int thuNotStarted = thu.Where(t => t.Status == (int)ItemTaskStatus.NotStarted).Count();
                int thuOnHold = thu.Where(t => t.Status == (int)ItemTaskStatus.OnHold).Count();
                int thuOverdue = thu.Where(t => t.Status == (int)ItemTaskStatus.Overdue).Count();

                var fri = taskQuery.ToList().Where(t => t.LastModifiedDateTime.DayOfWeek == DayOfWeek.Friday);
                int friCompleted = fri.Where(t => t.Status == (int)ItemTaskStatus.Completed).Count();
                int friInProgress = fri.Where(t => t.Status == (int)ItemTaskStatus.InProgress).Count();
                int friNotStarted = fri.Where(t => t.Status == (int)ItemTaskStatus.NotStarted).Count();
                int friOnHold = fri.Where(t => t.Status == (int)ItemTaskStatus.OnHold).Count();
                int friOverdue = fri.Where(t => t.Status == (int)ItemTaskStatus.Overdue).Count();

                var sat = taskQuery.ToList().Where(t => t.LastModifiedDateTime.DayOfWeek == DayOfWeek.Saturday);
                int satCompleted = sat.Where(t => t.Status == (int)ItemTaskStatus.Completed).Count();
                int satInProgress = sat.Where(t => t.Status == (int)ItemTaskStatus.InProgress).Count();
                int satNotStarted = sat.Where(t => t.Status == (int)ItemTaskStatus.NotStarted).Count();
                int satOnHold = sat.Where(t => t.Status == (int)ItemTaskStatus.OnHold).Count();
                int satOverdue = sat.Where(t => t.Status == (int)ItemTaskStatus.Overdue).Count();

                var sun = taskQuery.ToList().Where(t => t.LastModifiedDateTime.DayOfWeek == DayOfWeek.Sunday);
                int sunCompleted = sun.Where(t => t.Status == (int)ItemTaskStatus.Completed).Count();
                int sunInProgress = sun.Where(t => t.Status == (int)ItemTaskStatus.InProgress).Count();
                int sunNotStarted = sun.Where(t => t.Status == (int)ItemTaskStatus.NotStarted).Count();
                int sunOnHold = sun.Where(t => t.Status == (int)ItemTaskStatus.OnHold).Count();
                int sunOverdue = sun.Where(t => t.Status == (int)ItemTaskStatus.Overdue).Count();

                // Calculate statistics based on the selected period
                int total = taskQuery.Count();
                int completed = taskQuery.Where(t => t.Status == (int)ItemTaskStatus.Completed).Count();
                int inProgress = taskQuery.Where(t => t.Status == (int)ItemTaskStatus.InProgress).Count();
                int notStarted = taskQuery.Where(t => t.Status == (int)ItemTaskStatus.NotStarted).Count();
                int onHold = taskQuery.Where(t => t.Status == (int)ItemTaskStatus.OnHold).Count();
                int overdue = taskQuery.Where(t => t.Status == (int)ItemTaskStatus.Overdue).Count();

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        completeArray = new int[] { monCompleted, tueCompleted, wedCompleted, thuCompleted, friCompleted, satCompleted, sunCompleted },
                        inProgressArray = new int[] { monInProgress, tueInProgress, wedInProgress, thuInProgress, friInProgress, satInProgress, sunInProgress },
                        notStartedArray = new int[] { monNotStarted, tueNotStarted, wedNotStarted, thuNotStarted, friNotStarted, satNotStarted, sunNotStarted },
                        onHoldArray = new int[] { monOnHold, tueOnHold, wedOnHold, thuOnHold, friOnHold, satOnHold, sunOnHold },
                        overdueArray = new int[] { monOverdue, tueOverdue, wedOverdue, thuOverdue, friOverdue, satOverdue, sunOverdue },
                        //totalArray = new int[] {monTotal, tueTotal, wedTotal, thuTotal, friTotal, satTotal, sunTotal},
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tasks");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while fetching tasks"
                });
            }
        }

        public async Task<IActionResult> GetTasks(string type)
        {
            try
            {
                // Initialize query
                var query = dbContext.L1Tasks.Include(t => t.L1SubTasks).Where(t => t.CreatedByStudentId == LoginStudentId && t.Status != (int)ItemTaskStatus.Completed);

                // Get the start and end dates of the current week (Monday to Sunday)
                var today = DateTime.Today;
                var currentDayOfWeek = (int)today.DayOfWeek;
                var mondayOffset = currentDayOfWeek == 0 ? -6 : 1 - currentDayOfWeek;// Handle Sunday specially
                var currentWeekStart = today.AddDays(mondayOffset); // Monday
                var currentWeekEnd = currentWeekStart.AddDays(6); // Sunday
                var currentMonthStart = new DateTime(today.Year, today.Month, 1);
                var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

                if (type == "today")
                {
                    query = query.Where(t => 
                        // Tasks due today
                        (t.DueDate.Date == today && t.Status != (int)ItemTaskStatus.Completed) ||
                        // Include overdue tasks regardless of due date
                        (t.Status == (int)ItemTaskStatus.Overdue)
                    ).OrderByDescending(t => t.Priority)  // Highest priority first
                     .ThenBy(t => t.DueDate);            // Earlier due dates first
                }
                else if (type == "thisWeek")
                {
                    query = query.Where(t => 
                        // Tasks due this week
                        (t.DueDate >= currentWeekStart && t.DueDate <= currentWeekEnd && 
                         t.Status != (int)ItemTaskStatus.Completed) ||
                        // Include overdue tasks regardless of due date
                        (t.Status == (int)ItemTaskStatus.Overdue)
                    ).OrderByDescending(t => t.Priority)  // Highest priority first
                     .ThenBy(t => t.DueDate);            // Earlier due dates first
                }
                else if (type == "thisMonth")
                {
                    query = query.Where(t => 
                        // Tasks due this month
                        (t.DueDate >= currentMonthStart && t.DueDate <= currentMonthEnd && 
                         t.Status != (int)ItemTaskStatus.Completed) ||
                        // Include overdue tasks regardless of due date
                        (t.Status == (int)ItemTaskStatus.Overdue)
                    ).OrderByDescending(t => t.Priority)  // Highest priority first
                     .ThenBy(t => t.DueDate);            // Earlier due dates first
                }
                else if (type == "completed")
                {
                    query = query.Where(t => t.Status == (int)ItemTaskStatus.Completed)
                                .OrderByDescending(t => t.Priority)  // Highest priority first
                                .ThenBy(t => t.DueDate);            // Earlier due dates first
                }

                // Get paged results
                var tasks = await query
                    .Include(t => t.L1SubTasks)  // Include subtasks
                    .Select(t => new
                    {
                        id = t.Id,
                        title = t.Title,
                        description = string.IsNullOrEmpty(t.Description) ? string.Empty : t.Description,
                        category = t.Category == (int)ItemTaskCategory.Academic ? "Academic" :
                        t.Category == (int)ItemTaskCategory.Extracurricular ? "Extracurricular" :
                        t.Category == (int)ItemTaskCategory.PersonalDevelopment ? "Personal Development" :
                        t.Category == (int)ItemTaskCategory.Social ? "Social" :
                        t.Category == (int)ItemTaskCategory.HealthWellness ? "Health & Wellness" : "Miscellaneous",

                        statusDisplay = t.Status == (int)ItemTaskStatus.NotStarted ? "Not started" :
                        t.Status == (int)ItemTaskStatus.InProgress ? "In-progress" :
                        t.Status == (int)ItemTaskStatus.Completed ? "Completed" :
                        t.Status == (int)ItemTaskStatus.OnHold ? "On-hold" : "Overdue",

                        statuscss = t.Status == (int)ItemTaskStatus.NotStarted ? "notstarted" :
                        t.Status == (int)ItemTaskStatus.InProgress ? "inprogress" :
                        t.Status == (int)ItemTaskStatus.Completed ? "completed" :
                        t.Status == (int)ItemTaskStatus.OnHold ? "onhold" : "overdue",

                        priority = t.Priority == (int)PriorityLevel.Trivial ? "Trivial" :
                        t.Priority == (int)PriorityLevel.Low ? "Low" :
                        t.Priority == (int)PriorityLevel.Medium ? "Medium" :
                        t.Priority == (int)PriorityLevel.High ? "High" : "Critical",

                        prioritycss = t.Priority == (int)PriorityLevel.Trivial ? "trivial" :
                        t.Priority == (int)PriorityLevel.Low ? "low" :
                        t.Priority == (int)PriorityLevel.Medium ? "medium" :
                        t.Priority == (int)PriorityLevel.High ? "high" : "critical",

                        startDate = t.StartDate.ToString("yyyy-MM-dd"),
                        dueDate = t.DueDate.ToString("dd-MM-yyyy, hh:mm tt"),

                        dueDatecss = (t.DueDate - DateTime.Now).TotalDays <= 1 ? "deadline-urgent" :
                        (t.DueDate - DateTime.Now).TotalDays <= 3 ? "deadline-warning" : "deadline-safe",

                        isRecurring = t.IsRecurring,
                        recurringPresetId = t.L1RecurringPresetId,
                        isNotification = t.IsNotification,
                        notificationPresetId = t.L1NotificationPresetId,
                        completedSubtasksCount = t.L1SubTasks.Count(s => s.Status == (int)ItemTaskStatus.Completed),
                        totalSubtasksCount = t.L1SubTasks.Count,
                        progress = t.L1SubTasks.Any()
                            ? (t.L1SubTasks.Count(s => s.Status == (int)ItemTaskStatus.Completed) * 100 / t.L1SubTasks.Count)
                            : 0,
                        subtasks = t.L1SubTasks.Select(st => new
                        {
                            id = st.Id,
                            title = st.Title,
                            category = st.Category == (int)ItemTaskCategory.Academic ? "Academic" :
                            st.Category == (int)ItemTaskCategory.Extracurricular ? "Extracurricular" :
                            st.Category == (int)ItemTaskCategory.PersonalDevelopment ? "Personal Development" :
                            st.Category == (int)ItemTaskCategory.Social ? "Social" :
                            st.Category == (int)ItemTaskCategory.HealthWellness ? "Health & Wellness" : "Miscellaneous",

                            priorityDisplay = st.Priority == (int)PriorityLevel.Trivial ? "Trivial" :
                            st.Priority == (int)PriorityLevel.Low ? "Low" :
                            st.Priority == (int)PriorityLevel.Medium ? "Medium" :
                            st.Priority == (int)PriorityLevel.High ? "High" : "Critical",
                            prioritycss = st.Priority == (int)PriorityLevel.Trivial ? "trivial" :
                            st.Priority == (int)PriorityLevel.Low ? "low" :
                            st.Priority == (int)PriorityLevel.Medium ? "medium" :
                            st.Priority == (int)PriorityLevel.High ? "high" : "critical",
                            statusDisplay = st.Status == (int)ItemTaskStatus.NotStarted ? "Not started" :
                            st.Status == (int)ItemTaskStatus.InProgress ? "In-progress" :
                            st.Status == (int)ItemTaskStatus.Completed ? "Completed" :
                            st.Status == (int)ItemTaskStatus.OnHold ? "On-hold" : "Overdue",
                            statuscss = st.Status == (int)ItemTaskStatus.NotStarted ? "notstarted" :
                            st.Status == (int)ItemTaskStatus.InProgress ? "inprogress" :
                            st.Status == (int)ItemTaskStatus.Completed ? "completed" :
                            st.Status == (int)ItemTaskStatus.OnHold ? "onhold" : "overdue",
                            startDate = st.StartDate,
                            dueDate = st.DueDate.ToString("dd-MM-yyyy, hh:mm tt"),
                            dueDatecss = (st.DueDate - DateTime.Now).TotalDays <= 1 ? "overdue" :
                            (st.DueDate - DateTime.Now).TotalDays <= 3 ? "soon" : "deadline-safe",
                            isNotification = st.IsNotification,
                            notificationPresetId = st.L1NotificationPresetId,
                            defaultNotificationOptions = st.DefaultNotificationOptions,
                        }).ToList()
                    })
                    .ToListAsync();
                if (tasks.Count == 0)
                {
                    return Json(new
                    {
                        success = false
                    });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        tasks = tasks,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tasks");
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentForumPosts()
        {
            try
            {
                var recentPosts = await dbContext.L1DiscussionForums
                    .Where(p => p.CreatedByStudentId == LoginStudentId && p.Status != (int)ForumStatus.Deleted)
                    .OrderByDescending(p => p.CreatedDateTime)
                    .Take(5)
                    .Select(p => new
                    {
                        id = p.Id,
                        isNew = p.CreatedDateTime >= DateTime.Now.AddHours(-24),
                        isResolved =  p.Status == (int)ForumStatus.Resolved ? true : false,
                        title = p.Title,
                        description = p.Description,
                        createdDate = p.CreatedDateTime.ToString("dd-MM-yyyy, hh:mm tt"),
                        likeCount = p.LikeCount,
                        commentCount = p.CommentCount
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = recentPosts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching recent forum posts");
                return Json(new
                {
                    success = false,
                    message = "Error fetching forum posts"
                });
            }
        }
    }
}
