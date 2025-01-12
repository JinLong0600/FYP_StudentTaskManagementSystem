using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using StudentTaskManagement.Models;
using System.Security.Claims;
using WebApplication1.Controllers;

namespace StudentTaskManagement.Controllers
{
    public class _BaseController : Controller
    {
        protected readonly StudentTaskManagementContext dbContext;
        protected readonly ILogger _logger;
        private readonly UserManager<L1Students> _userManager;
        private readonly SignInManager<L1Students> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public _BaseController(StudentTaskManagementContext context, ILogger logger,
            UserManager<L1Students> userManager, SignInManager<L1Students> signInManager, IEmailService emailService, IWebHostEnvironment webHostEnvironment)
        {
            dbContext = context;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var student = await dbContext.L1Students
                .Where(s => s.Id == LoginStudentId)
                .Select(s => new {
                    s.UserName,
                    s.ProfileImage
                })
                .FirstOrDefaultAsync();

            ViewBag.StudentName = student?.UserName;
            ViewBag.ProfileImage = student?.ProfileImage;

            await base.OnActionExecutionAsync(context, next);
        }


        public string LoginStudentId
        {
            get
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    return userId;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public string LoginStudentUserName
        {
            get
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.Name);
                    return userId;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
