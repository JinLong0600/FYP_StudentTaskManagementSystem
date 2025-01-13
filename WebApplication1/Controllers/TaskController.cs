using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentTaskManagement.Models;
using StudentTaskManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using StudentTaskManagement.Controllers;
using Newtonsoft.Json;
using System.Security.Claims;
using static StudentTaskManagement.Utilities.GeneralEnum;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;


namespace WebApplication1.Controllers
{
    [Authorize]
    public class TaskController : _BaseController
    {
        protected readonly StudentTaskManagementContext dbContext;
        protected readonly ILogger _logger;
        private readonly UserManager<L1Students> _userManager;
        private readonly SignInManager<L1Students> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TaskController(StudentTaskManagementContext dbContext, ILogger<NotificationPresetController> logger, UserManager<L1Students> userManager, SignInManager<L1Students> signInManager, IEmailService emailService, IWebHostEnvironment webHostEnvironment)
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
            ViewData["ActiveMenu"] = "Tasks";
            return View();
        }

        [HttpGet]
        [Route("Task/CreateTask/{taskId?}")]
        public async Task<IActionResult> CreateTask(int? taskId)
        {
            L1NotificationPresets defaultNotification = dbContext.L1NotificationPresets.Where(x => x.IsSystemDefault && x.Status == (int)PresetPatternStatus.Active).FirstOrDefault();
            L1RecurringPatterns defaultRecurring = dbContext.L1RecurringPresets.Where(x => x.IsSystemDefault && x.Status == (int)PresetPatternStatus.Active).FirstOrDefault();
            // Get the list for dropdown and store in ViewBag
            ViewBag.SelectNotificationPresetList = GetNotificationPreset(defaultNotification);
            ViewBag.SelectRecurringPresetList = GetRecurringPreset(defaultRecurring);
            ViewData["ActiveMenu"] = "Tasks";

            if (taskId != null)
            {
                var task = await dbContext.L1Tasks
                .Include(t => t.L1SubTasks)  // Include subtasks
                .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound();
                }

                var viewModel = new L1TasksViewModel
                {
                    IsEdit = true,
                    // Map main task properties
                    TaskId = taskId.Value,
                    Title = task.Title,
                    Category = task.Category,
                    Description = task.Description,
                    StartDate = task.StartDate.ToString("dd-MM-yyyy hh:mm tt"),
                    DueDate = task.DueDate.ToString("dd-MM-yyyy hh:mm tt"),
                    Priority = task.Priority,
                    Status = task.Status,
                    IsRecurring = task.IsRecurring,
                    IsParentRecurring = task.IsParentRecurring,
                    L1RecurringPresetId = task.L1RecurringPresetId,
                    DefaultRecurringOptions = task.DefaultRecurringOptions,
                    IsNotification = task.IsNotification,
                    L1NotificationPresetId = task.L1NotificationPresetId,
                    DefaultNotificationOptions = task.DefaultNotificationOptions,
                    DefaultSystemL1NotificationPresetId = defaultNotification.Id,
                    DefaultSystemL1RecurringPresetId = defaultRecurring.Id,
                    // Map subtasks
                    SubtasksList = task.L1SubTasks.Select(s => new SubtaskListViewModel
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Category = s.Category,
                        Status = s.Status,
                        Priority = s.Priority,
                        StartDate = s.StartDate.ToString("dd-MM-yyyy hh:mm tt"),
                        DueDate = s.DueDate.ToString("dd-MM-yyyy hh:mm tt"),
                        IsNotification = s.IsNotification,
                        L1NotificationPresetId = s.L1NotificationPresetId,
                        DefaultNotificationOptions = s.DefaultNotificationOptions
                    }).ToList()
                };
                
                return View(viewModel);
            }
            else
            {
                var model = new L1TasksViewModel();
                model.IsEdit = false;
                model.DefaultSystemL1NotificationPresetId = defaultNotification.Id;
                model.DefaultSystemL1RecurringPresetId = defaultRecurring.Id;
                return View(model);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Add(L1TasksViewModel viewModel)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try 
            {
                var task = new L1Tasks
                {
                    Title = viewModel.Title,
                    Category = viewModel.Category,
                    Description = viewModel.Description,
                    Priority = viewModel.Priority,
                    Status = viewModel.Status,
                    StartDate = DateTime.Parse(viewModel.StartDate),
                    DueDate = DateTime.Parse(viewModel.DueDate),
                    IsRecurring = viewModel.IsRecurring,
                    IsParentRecurring = viewModel.IsRecurring ? true : null,
                    L1RecurringPresetId = viewModel.L1RecurringPresetId,
                    DefaultRecurringOptions = viewModel.DefaultRecurringOptions,
                    IsNotification = viewModel.IsNotification,
                    L1NotificationPresetId = viewModel.L1NotificationPresetId,
                    DefaultNotificationOptions = viewModel.DefaultNotificationOptions,
                    CreatedByStudentId = LoginStudentId,
                    LastModifiedDateTime = DateTime.Now,
                    DeletionDateTime = DateTime.Now,
                };
                await dbContext.L1Tasks.AddAsync(task);
                await dbContext.SaveChangesAsync();
                var subtaskList = JsonConvert.DeserializeObject<List<SubtaskListViewModel>>(viewModel.Subtasks);

                // Create subtasks if any exist
                if (subtaskList != null && subtaskList.Count() != 0)
                {
                    foreach (var item in subtaskList)
                    {
                        var subtask = new L1SubTasks
                        {
                            L1TaskId = task.Id, // Use the newly created task's ID
                            Title = item.Title,
                            Category = item.Category,
                            Priority = item.Priority,
                            Status = item.Status,
                            StartDate = DateTime.Parse(item.StartDate),
                            DueDate = DateTime.Parse(item.DueDate),
                            IsNotification = item.IsNotification,
                            L1NotificationPresetId = viewModel.L1NotificationPresetId,
                            DefaultNotificationOptions = viewModel.DefaultNotificationOptions,
                            CreatedByStudentId = LoginStudentId,
                            LastModifiedDateTime = DateTime.Now,
                            DeletionDateTime = DateTime.Now,
                            // Add other properties as needed
                        };

                        await dbContext.L1SubTasks.AddAsync(subtask);
                        await dbContext.SaveChangesAsync();
                    }


                }

                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Task and subtasks created successfully",
                    taskId = task.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification preset");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while creating the preset. Please try again."
                });
            }
        }

        [HttpPost]
        [Route("Task/Update/{taskId?}")]
        public async Task<IActionResult> Update(int taskId, L1TasksViewModel viewModel)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {

                // Get existing task with its subtasks
                var existingTask = await dbContext.L1Tasks
                    .Include(t => t.L1SubTasks)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (existingTask == null)
                {
                    return Json(new { success = false, message = "Task not found" });
                }

                // Update main task properties
                existingTask.Title = viewModel.Title;
                existingTask.Category = viewModel.Category;
                existingTask.Description = viewModel.Description;
                existingTask.Priority = viewModel.Priority;
                existingTask.Status = viewModel.Status;
                existingTask.StartDate = DateTime.Parse(viewModel.StartDate);
                existingTask.DueDate = DateTime.Parse(viewModel.DueDate);
                existingTask.IsRecurring = viewModel.IsRecurring;
                existingTask.IsParentRecurring = existingTask.IsParentRecurring == false ? false : viewModel.IsRecurring ? true : null;
                if (existingTask.L1RecurringPresetId != viewModel.L1RecurringPresetId)
                    existingTask.GeneratedCount = 0;
                existingTask.L1RecurringPresetId = viewModel.L1RecurringPresetId;
                existingTask.DefaultRecurringOptions = viewModel.DefaultRecurringOptions;
                existingTask.IsNotification = viewModel.IsNotification;
                existingTask.L1NotificationPresetId = viewModel.L1NotificationPresetId;
                existingTask.DefaultNotificationOptions = viewModel.DefaultNotificationOptions;
                existingTask.LastModifiedDateTime = DateTime.Now;

                // Parse updated subtasks from viewModel
                var updatedSubtasks = JsonConvert.DeserializeObject<List<SubtaskListViewModel>>(viewModel.Subtasks);

                if (updatedSubtasks != null)
                {
                    // Remove subtasks that are no longer in the updated list
                    var subtaskIdsToKeep = updatedSubtasks
                        .Where(s => s.Id.HasValue)
                        .Select(s => s.Id.Value)
                        .ToList();

                    var subtasksToDelete = existingTask.L1SubTasks
                        .Where(s => !subtaskIdsToKeep.Contains(s.Id))
                        .ToList();

                    foreach (var subtask in subtasksToDelete)
                    {
                        dbContext.L1SubTasks.Remove(subtask);
                    }

                    // Update existing and add new subtasks
                    foreach (var subtaskModel in updatedSubtasks)
                    {
                        if (subtaskModel.Id.HasValue)
                        {
                            // Update existing subtask
                            var existingSubtask = existingTask.L1SubTasks
                                .FirstOrDefault(s => s.Id == subtaskModel.Id);

                            if (existingSubtask != null)
                            {
                                existingSubtask.Title = subtaskModel.Title;
                                existingSubtask.Category = subtaskModel.Category;
                                existingSubtask.Priority = subtaskModel.Priority;
                                existingSubtask.Status = subtaskModel.Status;
                                existingSubtask.StartDate = DateTime.Parse(subtaskModel.StartDate);
                                existingSubtask.DueDate = DateTime.Parse(subtaskModel.DueDate);
                                existingSubtask.IsNotification = subtaskModel.IsNotification;
                                existingSubtask.L1NotificationPresetId = subtaskModel.L1NotificationPresetId;
                                existingSubtask.DefaultNotificationOptions = subtaskModel.DefaultNotificationOptions;
                                existingSubtask.LastModifiedDateTime = DateTime.Now;
                            }
                        }
                        else
                        {
                            // Add new subtask
                            var newSubtask = new L1SubTasks
                            {
                                L1TaskId = existingTask.Id,
                                Title = subtaskModel.Title,
                                Category = subtaskModel.Category,
                                Priority = subtaskModel.Priority,
                                Status = subtaskModel.Status,
                                StartDate = DateTime.Parse(subtaskModel.StartDate),
                                DueDate = DateTime.Parse(subtaskModel.DueDate),
                                IsNotification = subtaskModel.IsNotification,
                                L1NotificationPresetId = subtaskModel.L1NotificationPresetId,
                                DefaultNotificationOptions = subtaskModel.DefaultNotificationOptions,
                                CreatedByStudentId = LoginStudentId,
                                LastModifiedDateTime = DateTime.Now,
                                DeletionDateTime = DateTime.Now
                            };

                            await dbContext.L1SubTasks.AddAsync(newSubtask);
                        }
                    }
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Task and subtasks updated successfully",
                    taskId = existingTask.Id

                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating task and subtasks");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the task. Please try again."
                });
            }
        }

        public List<SelectListItem> GetNotificationPreset(L1NotificationPresets defaultNotification)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            // Add default option
            selectListItems.Add(new SelectListItem { Text = "- Please select -", Value = "" });

            // Get notification presets from database
            var notificationPresets = dbContext.L1NotificationPresets
                .Where(x => x.CreatedByStudentId == LoginStudentId && x.Status == (int)PresetPatternStatus.Active)
                .ToList();

            if (notificationPresets != null)
            {
                foreach (var preset in notificationPresets)
                {
                    selectListItems.Add(new SelectListItem
                    {
                        Text = preset.Name,  // Or whatever field you want to display
                        Value = preset.Id.ToString()
                    });
                }
            }

            selectListItems.OrderBy(x => x.Text).ToList();

            if (defaultNotification != null)
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = defaultNotification.Name,  // Or whatever field you want to display
                    Value = defaultNotification.Id.ToString()
                });
            }
            return selectListItems;
        }

        public List<SelectListItem> GetRecurringPreset(L1RecurringPatterns defaultRecurring)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            // Add default option
            selectListItems.Add(new SelectListItem { Text = "- Please select -", Value = "" });

            // Get notification presets from database
            var recurringPreset = dbContext.L1RecurringPresets
                .Where(x => x.CreatedByStudentId == LoginStudentId && x.Status == (int)PresetPatternStatus.Active)
                .ToList();

            if (recurringPreset != null)
            {
                foreach (var preset in recurringPreset)
                {
                    selectListItems.Add(new SelectListItem
                    {
                        Text = preset.Name,  // Or whatever field you want to display
                        Value = preset.Id.ToString()
                    });
                }
            }
            selectListItems.OrderBy(x => x.Text).ToList();
            if (defaultRecurring != null)
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = defaultRecurring.Name,  // Or whatever field you want to display
                    Value = defaultRecurring.Id.ToString()
                });
            }

            return selectListItems;
        }

        [HttpGet]
        public async Task<IActionResult> GetTaskCount()
        {
            try
            {
                // Get the start and end dates of the current week (Monday to Sunday)
                var today = DateTime.Today;
                var currentDayOfWeek = (int)today.DayOfWeek;
                var mondayOffset = currentDayOfWeek == 0 ? -6 : 1 - currentDayOfWeek;// Handle Sunday specially
                var currentWeekStart = today.AddDays(mondayOffset); // Monday
                var currentWeekEnd = currentWeekStart.AddDays(6); // Sunday


                // Initialize query
                int completedTask = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == LoginStudentId && t.Status == (int)ItemTaskStatus.Completed).Count();
                int thisWeekTask = await dbContext.L1Tasks

                    .Where(t => t.CreatedByStudentId == LoginStudentId
                           && t.Status != (int)ItemTaskStatus.Completed &&
                           (// This week's tasks
                            (t.DueDate >= currentWeekStart && t.DueDate <= currentWeekEnd) ||
                            // Overdue tasks (due date is in the past and status is overdue)
                            (t.DueDate < currentWeekStart && t.Status == (int)ItemTaskStatus.Overdue))).CountAsync();

                int upcomingTask = await dbContext.L1Tasks
                    .Where(t => t.CreatedByStudentId == LoginStudentId
                           && t.DueDate > currentWeekEnd  // Tasks due after this week
                           && t.Status != (int)ItemTaskStatus.Completed)
                    .CountAsync();

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        completedTask = completedTask,
                        thisWeekTask = thisWeekTask,
                        upcomingTask = upcomingTask,
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
        public async Task<IActionResult> GetTasks(string type, int page = 1, 
            string searchTerm = "", string category = "", string status = "", string priority = "", string startDate = "", string endDate = "")
        {
            try
            {
                // Get current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Initialize query
                var query = dbContext.L1Tasks.Include(t => t.L1SubTasks).Include(t => t.L1NotificationPresets)
                    .Include(t => t.L1NotificationPresets).Where(t => t.CreatedByStudentId == LoginStudentId); // TODO: Replace with actual userId

                // Get the start and end dates of the current week (Monday to Sunday)
                var today = DateTime.Today;
                var currentDayOfWeek = (int)today.DayOfWeek;
                var mondayOffset = currentDayOfWeek == 0 ? -6 : 1 - currentDayOfWeek;// Handle Sunday specially
                var currentWeekStart = today.AddDays(mondayOffset); // Monday
                var currentWeekEnd = currentWeekStart.AddDays(6); // Sunday

                if (type == "thisWeek")
                {
                    query = query.Where(t => t.Status != (int)ItemTaskStatus.Completed &&
                                        // This week's tasks
                                        (t.DueDate >= currentWeekStart && t.DueDate <= currentWeekEnd) ||
                                        // Overdue tasks (due date is in the past and status is overdue)
                                        (t.DueDate < currentWeekStart && t.Status == (int)ItemTaskStatus.Overdue)).OrderByDescending(t => t.Priority);
                }
                else if (type == "upcoming")
                {
                    query = query.Where(t => t.DueDate > currentWeekEnd && t.Status != (int)ItemTaskStatus.Completed).OrderByDescending(t => t.Priority);
                }
                else {
                    query = query.Where(t => t.Status == (int)ItemTaskStatus.Completed).OrderByDescending(t => t.Priority);
                }


                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(t =>
                        t.Title.Contains(searchTerm));
                }

                // Apply category filter
                if (!string.IsNullOrWhiteSpace(category))
                {
                    int categoryValue = int.Parse(category);
                    query = query.Where(t => t.Category == categoryValue);
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(status))
                {
                    int statusValue = int.Parse(status);
                    query = query.Where(t => t.Status == statusValue);
                }

                // Apply priority filter
                if (!string.IsNullOrWhiteSpace(priority))
                {
                    int priorityValue = int.Parse(priority);
                    query = query.Where(t => t.Priority == priorityValue);
                }

                // Apply startDate filter
                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    DateTime value = DateTime.Parse(startDate);
                    query = query.Where(t => t.DueDate.Date >= value);
                }

                // Apply endDate filter
                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    DateTime value = DateTime.Parse(endDate);
                    query = query.Where(t => t.DueDate.Date <= value);
                }

                // Set items per page
                int pageSize = 9;

                // Get total count for pagination
                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Ensure page number is valid
                page = Math.Max(1, Math.Min(page, totalPages));

                // Get paged results
                var tasks = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
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
                        isTaskNotificationPresetDeleted = t.L1NotificationPresets.Status == (int)PresetPatternStatus.Deleted ? true : false,
                        isTaskRecurringPatternDeleted = t.L1RecurringPatterns.Status == (int)PresetPatternStatus.Deleted ? true : false,
                        completedSubtasksCount = t.L1SubTasks.Count(s => s.Status == (int)ItemTaskStatus.Completed),
                        totalSubtasksCount = t.L1SubTasks.Count,
                        progress = t.L1SubTasks.Count != 0 ? (t.L1SubTasks.Count(s => s.Status == (int)ItemTaskStatus.Completed)) * 100 / t.L1SubTasks.Count : 0,
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
                        currentPage = page,
                        totalPages = totalPages,
                        totalItems = totalItems,
                        itemsPerPage = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tasks");
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

        private string TruncateDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return description;

            var words = description.Split(' ');
            if (words.Length <= 20)
                return description;

            return string.Join(" ", words.Take(20)) + "...";
        }

        [HttpGet]
        [Route("Task/GetTaskDetails/{taskId?}")]
        public async Task<IActionResult> GetTaskDetails(int taskId)
        {
            try
            {
                // Load everything in a single query with includes
                var existingTask = await dbContext.L1Tasks
                    .Include(t => t.L1SubTasks)
                    .Include(t => t.L1NotificationPresets)
                    .Include(t => t.L1RecurringPatterns)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (existingTask == null)
                    return NotFound();

                // No need for separate queries since we included everything
                var taskNotificationPreset = existingTask.L1NotificationPresets;
                var taskRecurringPattern = existingTask.L1RecurringPatterns;

                // Load subtask notification presets in a single query
                var subtaskIds = existingTask.L1SubTasks.Select(s => s.Id).ToList();
                var subtaskNotificationPresets = await dbContext.L1SubTasks
                    .Where(s => subtaskIds.Contains(s.Id))
                    .Include(s => s.L1NotificationPresets)
                    .ToDictionaryAsync(s => s.Id, s => s.L1NotificationPresets);

                var result = new
                {
                    task = new
                    {
                        taskId = existingTask.Id,
                        title = existingTask.Title,
                        description = string.IsNullOrEmpty(existingTask.Description) ? null : existingTask.Description,
                        category = existingTask.Category == (int)ItemTaskCategory.Academic ? "Academic" :
                        existingTask.Category == (int)ItemTaskCategory.Extracurricular ? "Extracurricular" :
                        existingTask.Category == (int)ItemTaskCategory.PersonalDevelopment ? "Personal Development" :
                        existingTask.Category == (int)ItemTaskCategory.Social ? "Social" :
                        existingTask.Category == (int)ItemTaskCategory.HealthWellness ? "Health & Wellness" : "Miscellaneous",

                        priorityDisplay = existingTask.Priority == (int)PriorityLevel.Trivial ? "Trivial" :
                        existingTask.Priority == (int)PriorityLevel.Low ? "Low" :
                        existingTask.Priority == (int)PriorityLevel.Medium ? "Medium" :
                        existingTask.Priority == (int)PriorityLevel.High ? "High" : "Critical",

                        prioritycss = existingTask.Priority == (int)PriorityLevel.Trivial ? "trivial" :
                        existingTask.Priority == (int)PriorityLevel.Low ? "low" :
                        existingTask.Priority == (int)PriorityLevel.Medium ? "medium" :
                        existingTask.Priority == (int)PriorityLevel.High ? "high" : "critical",

                        statusDisplay = existingTask.Status == (int)ItemTaskStatus.NotStarted ? "Not started" :
                        existingTask.Status == (int)ItemTaskStatus.InProgress ? "In-progress" :
                        existingTask.Status == (int)ItemTaskStatus.Completed ? "Completed" :
                        existingTask.Status == (int)ItemTaskStatus.OnHold ? "On-hold" : "Overdue",

                        statuscss = existingTask.Status == (int)ItemTaskStatus.NotStarted ? "notstarted" :
                        existingTask.Status == (int)ItemTaskStatus.InProgress ? "inprogress" :
                        existingTask.Status == (int)ItemTaskStatus.Completed ? "completed" :
                        existingTask.Status == (int)ItemTaskStatus.OnHold ? "onhold" : "overdue",
                        isParent = existingTask.IsParentRecurring,
                        startDate = existingTask.StartDate,
                        dueDate = existingTask.DueDate.ToString("dd-MM-yyyy, hh:mm tt"),
                        dueDatecss = (existingTask.DueDate - DateTime.Now).TotalDays <= 1 ? "overdue" :
                        (existingTask.DueDate - DateTime.Now).TotalDays <= 3 ? "due-soon" : "deadline-safe",
                        isRecurring = existingTask.IsRecurring,
                        recurringPresetId = existingTask.L1RecurringPresetId,
                        defaultRecurringOptions = existingTask.DefaultRecurringOptions,
                        isNotification = existingTask.IsNotification,
                        notificationPresetId = existingTask.L1NotificationPresetId,
                        defaultNotificationOptions = existingTask.DefaultNotificationOptions,
                        completedSubtasksCount = existingTask.L1SubTasks.Count(s => s.Status == (int)ItemTaskStatus.Completed),
                        totalSubtasksCount = existingTask.L1SubTasks.Count,
                    },
                    notificationPreset = taskNotificationPreset != null ? new
                    {
                        id = taskNotificationPreset.Id,
                        name = taskNotificationPreset.Name,
                        description = string.IsNullOrEmpty(taskNotificationPreset.Description) ? null : taskNotificationPreset.Description,
                        type = taskNotificationPreset.Type == (int)NotificationPresetType.Days ? "Days-based Preset" :
                         taskNotificationPreset.Type == (int)NotificationPresetType.Mintues ? "Minutes-based Preset" :
                         existingTask.DefaultNotificationOptions == 1 ? "Minutes-based Preset" :
                         existingTask.DefaultNotificationOptions == 2 ? "Minutes-based Preset" : "Days-based Preset",
                        defaultSettingMessagePills = existingTask.DefaultNotificationOptions == 1 ? "30 minutes before" :
                             existingTask.DefaultNotificationOptions == 2 ? "1 hour before" : "1 day before",
                        reminderDaysBefore = taskNotificationPreset.ReminderDaysBefore,
                        reminderHoursBefore = taskNotificationPreset.ReminderHoursBefore,
                        reminderMinutesBefore = taskNotificationPreset.ReminderMinutesBefore,
                        reminderTime = taskNotificationPreset.ReminderTime.HasValue ? taskNotificationPreset.ReminderTime.Value.ToString("hh:mm tt") : "",
                        isDaily = taskNotificationPreset.IsDaily,
                        isSystemDefault = taskNotificationPreset.IsSystemDefault,
                        isTaskNotificationPresetDeleted = taskNotificationPreset.Status == (int)PresetPatternStatus.Deleted ? true : false,
                    } : null,
                    recurringPreset = taskRecurringPattern != null ? new
                    {
                        id = taskRecurringPattern.Id,
                        name = taskRecurringPattern.Name,
                        description = string.IsNullOrEmpty(taskRecurringPattern.Description) ? null : taskRecurringPattern.Description,
                        type = taskRecurringPattern.Type == (int)RecurringType.Day ? "Recurs by Day(s)" :
                        taskRecurringPattern.Type == (int)RecurringType.Week ? "Recurs by Week(s)" :
                        taskRecurringPattern.Type == (int)RecurringType.BiWeek ? "Recurs by Bi-Weeks" :
                        taskRecurringPattern.Type == (int)RecurringType.Month ? "Recurs by Month(s)" : "Recurs by Bi-Months",

                        defaultSettingType = existingTask.DefaultRecurringOptions == 1 ? "Recurs by Day(s)" :
                        existingTask.DefaultRecurringOptions == 2 ? "Recurs by Week(s)" : "Recurs by Month(s)",

                        defaultSettingMessagePills = existingTask.DefaultRecurringOptions == 1 ? "Daily" :
                             existingTask.DefaultRecurringOptions == 2 ? "Weekly" : "Monthly",
                        daytoGenerate = getDisplayContext(taskRecurringPattern.Type, taskRecurringPattern.DaytoGenerate),
                        recurringCount = taskRecurringPattern.RecurringCount,
                        isSystemDefault = taskRecurringPattern.IsSystemDefault,
                        isTaskRecurringPatternDeleted = taskRecurringPattern.Status == (int)PresetPatternStatus.Deleted ? true : false,
                    } : null,
                    subtasks = existingTask.L1SubTasks.Select(st => new
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
                        notificationPreset = subtaskNotificationPresets.ContainsKey(st.Id) && st.IsNotification ? new
                        {
                            id = subtaskNotificationPresets[st.Id].Id,
                            name = subtaskNotificationPresets[st.Id].Name,
                            description = subtaskNotificationPresets[st.Id].Description,
                            //type = subtaskNotificationPresets[st.Id].Type == (int)NotificationPresetType.Days ? "Days-based Preset" : "Minutes-based Preset",
                            type = subtaskNotificationPresets[st.Id].Type == (int)NotificationPresetType.Days ? "Days-based Preset" :
                                 subtaskNotificationPresets[st.Id].Type == (int)NotificationPresetType.Mintues ? "Minutes-based Preset" :
                                 st.DefaultNotificationOptions == 1 ? "Minutes-based Preset" :
                                 st.DefaultNotificationOptions == 2 ? "Minutes-based Preset" : "Days-based Preset",

                            defaultSettingMessagePills = st.DefaultNotificationOptions == 1 ? "30 minutes before" :
                             st.DefaultNotificationOptions == 2 ? "1 hour before" : "1 day before",

                            reminderDaysBefore = subtaskNotificationPresets[st.Id].ReminderDaysBefore,
                            reminderHoursBefore = subtaskNotificationPresets[st.Id].ReminderHoursBefore,
                            reminderMinutesBefore = subtaskNotificationPresets[st.Id].ReminderMinutesBefore,
                            reminderTime = subtaskNotificationPresets[st.Id].ReminderTime.HasValue ? subtaskNotificationPresets[st.Id].ReminderTime.Value.ToString("hh:mm tt") : "",
                            isDaily = subtaskNotificationPresets[st.Id].IsDaily,
                            isSystemDefault = subtaskNotificationPresets[st.Id].IsSystemDefault,
                            isSubTaskNotificationPresetDeleted = subtaskNotificationPresets[st.Id].Status == (int)PresetPatternStatus.Deleted ? true : false,
                        } : null
                    }).ToList()
                };

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching task details");
                return Json(new { success = false, message = "An error occurred while fetching task details" });
            }
        }

        public static string getDisplayContext(int type, string dayToGenerate)
        {
            string context = string.Empty;

            if (type == (int)RecurringType.Day)
            {
                if (dayToGenerate == "0")
                {
                    context = "Recurs daily";
                }
                else
                {
                    context = "Recurs on every ";
                    if (dayToGenerate.Contains(','))
                    {
                        List<string> items = dayToGenerate.Split(',').ToList();
                        foreach (var item in items)
                        {
                            switch (item)
                            {
                                case "1":
                                    context += "Mon, ";
                                    break;
                                case "2":
                                    context += "Tue, ";
                                    break;
                                case "3":
                                    context += "Wed, ";
                                    break;
                                case "4":
                                    context += "Thu, ";
                                    break;
                                case "5":
                                    context += "Fri, ";
                                    break;
                                case "6":
                                    context += "Sat, ";
                                    break;
                                case "7":
                                    context += "Sun, ";
                                    break;
                            }
                        }

                    }
                    else
                    {
                        switch (dayToGenerate)
                        {
                            case "1":
                                context = "Monday";
                                break;
                            case "2":
                                context = "Tuesday";
                                break;
                            case "3":
                                context = "Wednesday";
                                break;
                            case "4":
                                context = "Thursday";
                                break;
                            case "5":
                                context = "Friday";
                                break;
                            case "6":
                                context = "Saturday";
                                break;
                            case "7":
                                context = "Sunday";
                                break;

                        }
                    }
                }
            }
            else if (type == (int)RecurringType.Week)
            {
                if (dayToGenerate == "0")
                {
                    context = "Recurs on same day";
                }
                else
                {
                    switch (dayToGenerate)
                    {
                        case "1":
                            context = "Recurs on weekly' Monday";
                            break;
                        case "2":
                            context = "Recurs on weekly' Tuesday";
                            break;
                        case "3":
                            context = "Recurs on weekly' Wednesday";
                            break;
                        case "4":
                            context = "Recurs on weekly' Thursday";
                            break;
                        case "5":
                            context = "Recurs on weekly' Friday";
                            break;
                        case "6":
                            context = "Recurs on weekly' Saturday";
                            break;
                        case "7":
                            context = "Recurs on weekly' Sunday";
                            break;
                    }
                }
            }
            else if (type == (int)RecurringType.BiWeek)
            {
                context = "Recurs on every two weeks";
            }
            else if (type == (int)RecurringType.Month)
            {
                if (dayToGenerate == "0")
                {
                    context = "Recurs monthly";
                }
                else
                {
                    context = "Recurs on every ";
                    if (dayToGenerate.Contains(','))
                    {
                        List<string> items = dayToGenerate.Split(',').ToList();
                        foreach (var item in items)
                        {
                            switch (item)
                            {
                                case "1":
                                    context += "Jan, ";
                                    break;
                                case "2":
                                    context += "Feb, ";
                                    break;
                                case "3":
                                    context += "Mar, ";
                                    break;
                                case "4":
                                    context += "Apr, ";
                                    break;
                                case "5":
                                    context += "May, ";
                                    break;
                                case "6":
                                    context += "Jun, ";
                                    break;
                                case "7":
                                    context += "Jul, ";
                                    break;
                                case "8":
                                    context += "Aug, ";
                                    break;
                                case "9":
                                    context += "Sep, ";
                                    break;
                                case "10":
                                    context += "Oct, ";
                                    break;
                                case "11":
                                    context += "Nov, ";
                                    break;
                                case "12":
                                    context += "Dec, ";
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                context = "Recurring every two months";
            }
            return context.TrimEnd(' ').TrimEnd(',');
        }

        [HttpPost]
        [Route("Task/AjaxDeleteTask/{taskId?}")]
        public async Task<IActionResult> AjaxDeleteTask(int taskId)
        {
            try
            {
                using var transaction = await dbContext.Database.BeginTransactionAsync();
                
                var task = await dbContext.L1Tasks
                    .Include(t => t.L1SubTasks)
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.CreatedByStudentId == LoginStudentId);

                if (task == null)
                {
                    return Json(new { success = false, message = "Task not found" });
                }

                // Delete all subtasks first
                if (task.L1SubTasks != null && task.L1SubTasks.Any())
                {
                    dbContext.L1SubTasks.RemoveRange(task.L1SubTasks);
                }

                // Delete the main task
                dbContext.L1Tasks.Remove(task);
                
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { 
                    success = true, 
                    message = "Task and all related subtasks have been deleted successfully" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task");
                return Json(new { 
                    success = false, 
                    message = "An error occurred while deleting the task" 
                });
            }
        }
    }

}



