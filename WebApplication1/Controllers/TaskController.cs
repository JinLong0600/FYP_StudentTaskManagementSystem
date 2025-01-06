﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentTaskManagement.Models;
using StudentTaskManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using Humanizer.Localisation;
using StudentTaskManagement.Controllers;
using Newtonsoft.Json;
using System.Security.Claims;
using static StudentTaskManagement.Utilities.GeneralEnum;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MailKit.Search;


namespace WebApplication1.Controllers
{
    public class TaskController : Controller
    {
        private readonly StudentTaskManagementContext dbContext;
        private readonly ILogger<NotificationPresetController> _logger;

        public TaskController(StudentTaskManagementContext dbContext,
            ILogger<NotificationPresetController> logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
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
            // Get the list for dropdown and store in ViewBag
            ViewBag.SelectNotificationPresetList = GetNotificationPreset();
            ViewBag.SelectRecurringPresetList = GetRecurringPreset();
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
                    StartDate = task.StartDate,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    Status = task.Status,
                    IsRecurring = task.IsRecurring,
                    L1RecurringPresetId = task.L1RecurringPresetId,
                    DefaultRecurringOptions = task.DefaultRecurringOptions,
                    IsNotification = task.IsNotification,
                    L1NotificationPresetId = task.L1NotificationPresetId,
                    DefaultNotificationOptions = task.DefaultNotificationOptions,
                    // Map subtasks
                    SubtasksList = task.L1SubTasks.Select(s => new SubtaskListViewModel
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Category = s.Category,
                        Status = s.Status,
                        Priority = s.Priority,
                        StartDate = s.StartDate,
                        DueDate = s.DueDate,
                        Description = s.Description,
                        IsNotification = s.IsNotification,
                        L1NotificationPresetId = s.L1NotificationPresetId,
                        DefaultNotificationOptions = s.DefaultNotificationOptions
                    }).ToList()
                };

                // Add any necessary ViewBag data
                ViewBag.SelectNotificationPresetList = GetNotificationPreset();
                ViewBag.SelectRecurringPresetList = GetRecurringPreset();
                
                return View(viewModel);
            }
            else
            {
                var model = new L1TasksViewModel();
                model.IsEdit = false;
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
                    StartDate = viewModel.StartDate,
                    DueDate = viewModel.DueDate,
                    IsRecurring = viewModel.IsRecurring,
                    L1RecurringPresetId = viewModel.L1RecurringPresetId,
                    DefaultRecurringOptions = viewModel.DefaultRecurringOptions,
                    IsNotification = viewModel.IsNotification,
                    L1NotificationPresetId = viewModel.L1NotificationPresetId,
                    DefaultNotificationOptions = viewModel.DefaultNotificationOptions,
                    CreatedByStudentId = "1",//userId,
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
                            Description = item.Description,
                            Priority = item.Priority,
                            Status = item.Status,
                            StartDate = item.StartDate,
                            DueDate = item.DueDate,
                            IsNotification = item.IsNotification,
                            L1NotificationPresetId = viewModel.L1NotificationPresetId,
                            DefaultNotificationOptions = viewModel.DefaultNotificationOptions,
                            CreatedByStudentId = "1",//userId,
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
                    message = "Task and subtasks created successfully"
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
                existingTask.StartDate = viewModel.StartDate;
                existingTask.DueDate = viewModel.DueDate;
                existingTask.IsRecurring = viewModel.IsRecurring;
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
                                existingSubtask.Description = subtaskModel.Description;
                                existingSubtask.Priority = subtaskModel.Priority;
                                existingSubtask.Status = subtaskModel.Status;
                                existingSubtask.StartDate = subtaskModel.StartDate;
                                existingSubtask.DueDate = subtaskModel.DueDate;
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
                                Description = subtaskModel.Description,
                                Priority = subtaskModel.Priority,
                                Status = subtaskModel.Status,
                                StartDate = subtaskModel.StartDate,
                                DueDate = subtaskModel.DueDate,
                                IsNotification = subtaskModel.IsNotification,
                                L1NotificationPresetId = subtaskModel.L1NotificationPresetId,
                                DefaultNotificationOptions = subtaskModel.DefaultNotificationOptions,
                                CreatedByStudentId = "1",//userId,
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
                    message = "Task and subtasks updated successfully"
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

        public List<SelectListItem> GetNotificationPreset()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            // Add default option
            selectListItems.Add(new SelectListItem { Text = "- Please select -", Value = "" });

            // Get notification presets from database
            var notificationPresets = dbContext.L1NotificationPresets
                .Where(x => x.CreatedByStudentId == 1)
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
            selectListItems.Add(new SelectListItem { Text = "Custom", Value = "0" });
            return selectListItems.OrderBy(x => x.Text).ToList();
        }

        public List<SelectListItem> GetRecurringPreset()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            // Add default option
            selectListItems.Add(new SelectListItem { Text = "- Please select -", Value = "" });

            // Get notification presets from database
            var recurringPreset = dbContext.L1RecurringPresets
                .Where(x => x.CreatedByStudentId == 1)  // Optionally filter only active presets
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
            selectListItems.Add(new SelectListItem { Text = "Custom", Value = "0" });

            return selectListItems;
        }

        [HttpGet]
        public async Task<IActionResult> GetTaskCount()
        {
            try
            {
                // Get current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Get the start and end dates of the current week (Monday to Sunday)
                var today = DateTime.Today;
                var currentDayOfWeek = (int)today.DayOfWeek;
                var mondayOffset = currentDayOfWeek == 0 ? -6 : 1 - currentDayOfWeek;// Handle Sunday specially
                var currentWeekStart = today.AddDays(mondayOffset); // Monday
                var currentWeekEnd = currentWeekStart.AddDays(6); // Sunday


                // Initialize query
                int completedTask = dbContext.L1Tasks.Where(t => t.CreatedByStudentId == "1" && t.Status == (int)ItemTaskStatus.Completed).Count();
                int thisWeekTask = await dbContext.L1Tasks
                    .Where(t => t.CreatedByStudentId == "1"
                           && t.Status != (int)ItemTaskStatus.Completed
                           && t.DueDate >= currentWeekStart
                           && t.DueDate <= currentWeekEnd)
                    .CountAsync();

                int upcomingTask = await dbContext.L1Tasks
                    .Where(t => t.CreatedByStudentId == "1"
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
        public async Task<IActionResult> GetTasks(string type, int page = 1, string searchTerm = "", string status = "", string priority = "")
        {
            try
            {
                // Get current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Initialize query
                var query = dbContext.L1Tasks.Include(t => t.L1SubTasks).Where(t => t.CreatedByStudentId == "1"); // TODO: Replace with actual userId

                // Get the start and end dates of the current week (Monday to Sunday)
                var today = DateTime.Today;
                var currentDayOfWeek = (int)today.DayOfWeek;
                var mondayOffset = currentDayOfWeek == 0 ? -6 : 1 - currentDayOfWeek;// Handle Sunday specially
                var currentWeekStart = today.AddDays(mondayOffset); // Monday
                var currentWeekEnd = currentWeekStart.AddDays(6); // Sunday

                if (type == "thisWeek")
                {
                    query = query.Where(t => t.Status != (int)ItemTaskStatus.Completed
                                           && t.DueDate >= currentWeekStart
                                           && t.DueDate <= currentWeekEnd).OrderByDescending(t => t.Priority);
                }
                else if (type == "upcoming")
                {
                    query = query.Where(t => t.DueDate > currentWeekEnd && t.Status != (int)ItemTaskStatus.Completed).OrderByDescending(t => t.Priority); ;
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

                        startDate = t.StartDate.HasValue? t.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                        dueDate = t.DueDate.ToString("yyyy-MM-dd"),

                        dueDatecss = (t.DueDate - DateTime.Now).TotalDays <= 1 ? "deadline-urgent" :
                        (t.DueDate - DateTime.Now).TotalDays <= 3 ? "deadline-warning" :
                        (t.L1SubTasks.Count(s => s.Status == (int)ItemTaskStatus.Completed)) == t.L1SubTasks.Count ? "completed": "deadline-safe",

                        isRecurring = t.IsRecurring,
                        recurringPresetId = t.L1RecurringPresetId,
                        isNotification = t.IsNotification,
                        notificationPresetId = t.L1NotificationPresetId,
                        completedSubtasksCount = t.L1SubTasks.Count(s => s.Status == (int)ItemTaskStatus.Completed),
                        totalSubtasksCount = t.L1SubTasks.Count,
                        progress = t.L1SubTasks.Any() 
                            ? (t.L1SubTasks.Count(s => s.Status == 2) * 100 / t.L1SubTasks.Count)
                            : 0
                    })
                    .ToListAsync();

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
                return Json(new
                {
                    success = false,
                    message = "An error occurred while fetching tasks"
                });
            }
        }

        private string TruncateDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return description;

            var words = description.Split(' ');
            if (words.Length <= 15)
                return description;

            return string.Join(" ", words.Take(15)) + "...";
        }

        [HttpGet]
        [Route("Task/GetTaskDetails/{taskId?}")]
        public async Task<IActionResult> GetTaskDetails(int taskId)
        {
            try
            {
                var existingTask = await dbContext.L1Tasks
                    .Include(t => t.L1SubTasks)  // Include subtasks
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (existingTask == null)
                    return NotFound();

                // Get notification preset for main task
                var taskNotificationPreset = existingTask.L1NotificationPresetId.HasValue 
                    ? await dbContext.L1NotificationPresets.FirstOrDefaultAsync(n => n.Id == existingTask.L1NotificationPresetId)
                    : null;

                // Get recurring preset for main task
                var taskRecurringPreset = existingTask.L1RecurringPresetId.HasValue
                    ? await dbContext.L1RecurringPresets.FirstOrDefaultAsync(r => r.Id == existingTask.L1RecurringPresetId)
                    : null;

                var result = new
                {
                    task = new
                    {
                        taskId = existingTask.Id,
                        title = existingTask.Title,
                        description = string.IsNullOrEmpty(existingTask.Description) ? "No description provided" : existingTask.Description,
                        category = existingTask.Category,
                        priority = existingTask.Priority,
                        status = existingTask.Status,
                        startDate = existingTask.StartDate,
                        dueDate = existingTask.DueDate,
                        isRecurring = existingTask.IsRecurring,
                        recurringPresetId = existingTask.L1RecurringPresetId,
                        defaultRecurringOptions = existingTask.DefaultRecurringOptions,
                        isNotification = existingTask.IsNotification,
                        notificationPresetId = existingTask.L1NotificationPresetId,
                        defaultNotificationOptions = existingTask.DefaultNotificationOptions
                    },
                    notificationPreset = taskNotificationPreset != null ? new
                    {
                        id = taskNotificationPreset.Id,
                        name = taskNotificationPreset.Name,
                        description = taskNotificationPreset.Description,
                        type = taskNotificationPreset.Type,
                    } : null,
                    recurringPreset = taskRecurringPreset != null ? new
                    {
                        id = taskRecurringPreset.Id,
                        name = taskRecurringPreset.Name,
                        description = taskRecurringPreset.Description,
                        type = taskRecurringPreset.Type,
                    } : null,
                    subtasks = (await Task.WhenAll(existingTask.L1SubTasks.Select(async st => 
                    {
                        var subtaskNotificationPreset = st.L1NotificationPresetId.HasValue
                            ? await dbContext.L1NotificationPresets.FirstOrDefaultAsync(n => n.Id == st.L1NotificationPresetId)
                            : null;

                        return new
                        {
                            id = st.Id,
                            title = st.Title,
                            description = st.Description,
                            category = st.Category,
                            priority = st.Priority,
                            status = st.Status,
                            startDate = st.StartDate,
                            dueDate = st.DueDate,
                            isNotification = st.IsNotification,
                            notificationPresetId = st.L1NotificationPresetId,
                            defaultNotificationOptions = st.DefaultNotificationOptions,
                            notificationPreset = subtaskNotificationPreset != null ? new
                            {
                                id = subtaskNotificationPreset.Id,
                                name = subtaskNotificationPreset.Name,
                                description = subtaskNotificationPreset.Description,
                                type = subtaskNotificationPreset.Type,
                            } : null
                        };
                    })
                    )).ToList()
                };

                if (result == null)
                {
                    return Json(new { success = false, message = "Task not found" });
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching task details");
                return Json(new { success = false, message = "An error occurred while fetching task details" });
            }
        }
    }

}



