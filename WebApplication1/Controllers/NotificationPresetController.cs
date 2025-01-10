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
using Microsoft.AspNetCore.Authorization;


namespace StudentTaskManagement.Controllers
{
    [Authorize]
    public class NotificationPresetController : _BaseController
    {
        private readonly StudentTaskManagementContext dbContext;
        private readonly ILogger<NotificationPresetController> _logger;

        public NotificationPresetController(StudentTaskManagementContext dbContext,
            ILogger<NotificationPresetController> logger)
        { 
            this.dbContext = dbContext;
            this._logger = logger;
        }

        // GET: ReminderSettingController
        public ActionResult Index()
        {
            ViewData["ActiveMenu"] = "Notification";
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> AjaxCreate(L1NotificationPresetsViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { 
                        success = false, 
                        message = "Please check your input and try again.",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                // Create preset object from view model
                var preset = new L1NotificationPresets
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Type = viewModel.Type,
                    IsDaily = viewModel.IsDaily,
                    ReminderDaysBefore = viewModel.Type == (int)NotificationPresetType.Days ? viewModel.ReminderDaysBefore : null,
                    ReminderHoursBefore = viewModel.Type == (int)NotificationPresetType.Mintues ? viewModel.ReminderHoursBefore : null,
                    ReminderMinutesBefore = viewModel.Type == (int)NotificationPresetType.Mintues ? viewModel.ReminderMinutesBefore : null,
                    ReminderTime = viewModel.ReminderTime,
                    CreatedByStudentId = LoginStudentId,
                    LastModifiedDateTime = DateTime.Now,
                    Status = (int)PresetPatternStatus.Active,
                };

                // Save to database
                await dbContext.AddAsync(preset);
                await dbContext.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Notification preset created successfully.",
                    data = new {
                        id = preset.Id,
                        name = preset.Name
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification preset");
                return Json(new { 
                    success = false, 
                    message = "An error occurred while creating the preset. Please try again."
                });
            }
        }

        #region Read & Update
        [HttpGet]
        public async Task<IActionResult> GetPresets(int page = 1, string searchTerm = "", string type = "")
        {
            try
            {
                // Initialize query
                var query = dbContext.L1NotificationPresets
                    .Include(np => np.L1Tasks)
                    .Include(np => np.L1SubTasks)
                    .Where(np => np.CreatedByStudentId == LoginStudentId && np.Status == (int)PresetPatternStatus.Active);

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(np =>
                        np.Name.Contains(searchTerm));
                }

                // Apply type filter
                if (!string.IsNullOrWhiteSpace(type))
                {
                    query = query.Where(np => np.Type == Convert.ToInt32(type));
                }

                // Set items per page
                int pageSize = 6;

                // Get total count for pagination
                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Ensure page number is valid
                page = Math.Max(1, Math.Min(page, totalPages));

                // Get paged results
                var presets = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(np => new
                    {
                        id = np.Id,
                        name = np.Name,
                        description = string.IsNullOrEmpty(np.Description) ? "- No Description -" : np.Description,
                        type = np.Type == (int)NotificationPresetType.Days ? "Days-based Notification Preset" : "Minutes-based Notification Preset",
                        reminderDaysBefore = np.ReminderDaysBefore,
                        reminderHoursBefore = np.ReminderHoursBefore,
                        reminderMinutesBefore = np.ReminderMinutesBefore,
                        reminderTime =  np.ReminderTime.HasValue ? np.ReminderTime.Value.ToString("hh:mm tt") : "",
                        isDaily = np.IsDaily,
                        activeTasksCount = np.L1Tasks.Where(t => t.Status != 0).Count(),
                        subItemsCount = np.L1SubTasks.Count,
                    })
                    .ToListAsync();

                if (presets.Count == 0)
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
                        presets = presets,
                        currentPage = page,
                        totalPages = totalPages,
                        totalItems = totalItems,
                        itemsPerPage = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching notification presets");
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AjaxGetPresetDetails(int id)
        {
            try
            {
                /*              var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                                if (string.IsNullOrEmpty(userId))
                                {
                                    return Json(new { success = false, message = "User not authenticated" });
                                }*/

                var preset = await dbContext.L1NotificationPresets
                                    .Where(np => np.Id == id && np.CreatedByStudentId == LoginStudentId)
                                    .Select(np => new
                                    {
                                        id = np.Id,
                                        name = np.Name,
                                        description = np.Description,
                                        type = np.Type,
                                        daysBefore = np.ReminderDaysBefore,
                                        hoursBefore = np.ReminderHoursBefore,
                                        minutesBefore = np.ReminderMinutesBefore,
                                        reminderTime = np.ReminderTime.HasValue ? np.ReminderTime.Value.ToString(@"hh\:mm") : "",
                                        isDaily = np.IsDaily
                                    })
                                    .FirstOrDefaultAsync();

                if (preset == null)
                {
                    return Json(new { success = false, message = "Preset not found" });
                }

                return Json(new { success = true, data = preset });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching preset details");
                return Json(new { success = false, message = "An error occurred while fetching preset details" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AjaxEdit( L1NotificationPresetsViewModel viewModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
/*                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }*/

                var existingPreset = await dbContext.L1NotificationPresets
                    .FirstOrDefaultAsync(np => np.Id == viewModel.Id && np.CreatedByStudentId == LoginStudentId);

                if (existingPreset == null)
                {
                    return Json(new { success = false, message = "Preset not found" });
                }

                // Update the existing preset
                existingPreset.Name = viewModel.Name;
                existingPreset.Description = viewModel.Description;
                existingPreset.Type = viewModel.Type;
                existingPreset.ReminderDaysBefore = viewModel.Type == (int)NotificationPresetType.Days ? viewModel.ReminderDaysBefore : null;
                existingPreset.ReminderHoursBefore = viewModel.Type == (int)NotificationPresetType.Mintues ? viewModel.ReminderHoursBefore : null;
                existingPreset.ReminderMinutesBefore = viewModel.Type == (int)NotificationPresetType.Mintues ? viewModel.ReminderMinutesBefore : null;
                existingPreset.ReminderTime = viewModel.Type == (int)NotificationPresetType.Days ? viewModel.ReminderTime : default;
                existingPreset.IsDaily = viewModel.IsDaily;
                existingPreset.LastModifiedDateTime = DateTime.Now;

                await dbContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Preset updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating preset");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the preset"
                });
            }
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> AjaxDelete(int id)
        {
            try
            {
                // Find the preset and verify ownership
                var preset = await dbContext.L1NotificationPresets
                    .FirstOrDefaultAsync(np => np.Id == id && np.CreatedByStudentId == LoginStudentId);
                if (preset == null)
                {
                    return Json(new { success = false, message = "Preset not found or you don't have permission to delete it" });
                }
                // Remove the preset

                preset.Status = (int)PresetPatternStatus.Deleted;
                preset.DeletionDateTime = DateTime.Now;
                await dbContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Preset deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting preset {PresetId}", id);
                return Json(new
                {
                    success = false,
                    message = "An error occurred while deleting the preset"
                });
            }

        }
    }

}
