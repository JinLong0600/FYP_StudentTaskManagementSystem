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


namespace StudentTaskManagement.Controllers
{
    public class RecurringPresetController : Controller
    {
        private readonly StudentTaskManagementContext dbContext;
        private readonly ILogger<RecurringPresetController> _logger;

        public RecurringPresetController(StudentTaskManagementContext dbContext,
            ILogger<RecurringPresetController> logger)
        { 
            this.dbContext = dbContext;
            this._logger = logger;
        }

        // GET: ReminderSettingController
        public ActionResult Index()
        {
            ViewData["ActiveMenu"] = "Recurring";
            return View();
        }

        // GET: ReminderSettingController/Create
        [HttpPost]
        public async Task<IActionResult> AjaxCreate(L1RecurringPresetViewModel viewModel)
        {
            try
            {

                // Create preset object from view model
                var preset = new L1RecurringPatterns
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Type = viewModel.Type,
                    DaytoGenerate = viewModel.DaytoGenerateHidden.Contains("0,") ? "0" : viewModel.DaytoGenerateHidden,
                    RecurringCount = viewModel.RecurringCount,
                    Status = (int)PresetStatus.Active,
                    CreatedByStudentId = 1, // If using authentication
                    LastModifiedDateTime = DateTime.Now
                };

                // Save to database
                await dbContext.AddAsync(preset);
                await dbContext.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Recurring pattern has been created successfully.",
                    data = new {
                        id = preset.Id,
                        name = preset.Name
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recurring pattern");
                return Json(new { 
                    success = false, 
                    message = "An error occurred while creating the pattern. Please try again."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPresets(int page = 1, string searchTerm = "", string type = "")
        {
            try
            {
                // Get current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
/*                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }*/

                // Initialize query
                var query = dbContext.L1RecurringPresets
/*                  .Include(np => np.Tasks)
                    .Include(np => np.SubTasks)*/
                    .Where(np => np.CreatedByStudentId == 1 && np.Status == (int)PresetStatus.Active);

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
                        description = string.IsNullOrEmpty(np.Description) ? string.Empty : np.Description,
                        type = np.Type == (int)RecurringType.Day ? "Recurring Pattern by Day(s)" :
                        np.Type == (int)RecurringType.Week ? "Recurring Pattern by Week(s)" :
                        np.Type == (int)RecurringType.BiWeek ? "Recurring Pattern by Bi-Weeks" :
                        np.Type == (int)RecurringType.Month ? "Recurring Pattern by Month(s)" : "Recurring Pattern by Bi-Months",
                        dayToGenerate = getDisplayContext(np.Type, np.DaytoGenerate),
                        recurringCount = np.RecurringCount.HasValue ? np.RecurringCount.ToString() : string.Empty,
                    })
                    .ToListAsync();

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
                return Json(new
                {
                    success = false,
                    message = "An error occurred while fetching notification presets"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AjaxGetPresetDetails(int id)
        {
            try
            {
                /*              
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                    {
                        return Json(new { success = false, message = "User not authenticated" });
                    }
                */

                var preset = await dbContext.L1RecurringPresets
                                    .Where(np => np.Id == id && np.CreatedByStudentId == 1)
                                    .Select(np => new
                                    {
                                        id = np.Id,
                                        name = np.Name,
                                        description = np.Description,
                                        type = np.Type,
                                        dayToGenerate = np.DaytoGenerate,
                                        recurringCount = np.RecurringCount
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
        public async Task<IActionResult> AjaxEdit(L1RecurringPresetViewModel viewModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
/*              if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }*/

                var existingPreset = await dbContext.L1RecurringPresets
                    .FirstOrDefaultAsync(np => np.Id == viewModel.Id && np.CreatedByStudentId == 1);

                if (existingPreset == null)
                {
                    return Json(new { success = false, message = "Preset not found" });
                }

                // Update the existing preset
                existingPreset.Name = viewModel.Name;
                existingPreset.Description = viewModel.Description;
                existingPreset.Type = viewModel.Type;

                existingPreset.LastModifiedDateTime = DateTime.Now;

                await dbContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Recurring pattern has been created successfully.",
                    data = new
                    {
                        id = existingPreset.Id,
                        name = existingPreset.Name
                    }
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

        [HttpPost]
        public async Task<IActionResult> AjaxDelete(int id)
        {
            try
            {
                // Get current user ID
/*                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }*/
                // Find the preset and verify ownership
                var preset = await dbContext.L1RecurringPresets
                    .FirstOrDefaultAsync(np => np.Id == id && np.CreatedByStudentId == 1);
                if (preset == null)
                {
                    return Json(new { success = false, message = "Preset not found or you don't have permission to delete it" });
                }
                // Change Status to preset
                preset.Status = (int)PresetStatus.Deleted;
                preset.DeletionDateTime = DateTime.Now;

                dbContext.L1RecurringPresets.Update(preset);
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

        public static string getDisplayContext(int type, string dayToGenerate) {
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
                                    context += "Mon,";
                                    break;
                                case "2":
                                    context += "Tue,";
                                    break;
                                case "3":
                                    context += "Wed,";
                                    break;
                                case "4":
                                    context += "Thu,";
                                    break;
                                case "5":
                                    context += "Fri,";
                                    break;
                                case "6":
                                    context += "Sat,";
                                    break;
                                case "7":
                                    context += "Sun,";
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
                            context = "Recurs on Monday weekly";
                            break;
                        case "2":
                            context = "Recurs on Tuesday weekly";
                            break;
                        case "3":
                            context = "Recurs on Wednesday weekly";
                            break;
                        case "4":
                            context = "Recurs on Thursday weekly";
                            break;
                        case "5":
                            context = "Recurs on Friday weekly";
                            break;
                        case "6":
                            context = "Recurs on Saturday weekly";
                            break;
                        case "7":
                            context = "Recurs on Sunday weekly";
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
                            context = "Recurs on every";
                            switch (item)
                            {
                                case "1":
                                    context = "Jan";
                                    break;
                                case "2":
                                    context = "Feb";
                                    break;
                                case "3":
                                    context = "Mar";
                                    break;
                                case "4":
                                    context = "Apr";
                                    break;
                                case "5":
                                    context = "May";
                                    break;
                                case "6":
                                    context = "Jun";
                                    break;
                                case "7":
                                    context = "Jul";
                                    break;
                                case "8":
                                    context = "Aug";
                                    break;
                                case "9":
                                    context = "Sep";
                                    break;
                                case "10":
                                    context = "Oct";
                                    break;
                                case "11":
                                    context = "Nov";
                                    break;
                                case "12":
                                    context = "Dec";
                                    break;
                            }
                        }
                    }
                }
            }
            else {
                context = "Recurring every two months";
            }
            return context.TrimEnd(',');
        }
    }

}
