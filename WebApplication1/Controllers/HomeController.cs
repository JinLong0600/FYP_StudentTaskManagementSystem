using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentTaskManagement.Models;
using StudentTaskManagement.Security;
using StudentTaskManagement.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<L1Students> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(
            UserManager<L1Students> userManager,
            ILogger<HomeController> logger,
            IWebHostEnvironment webHostEnvironment
            )
        {
            this._userManager = userManager;
            this._logger = logger;
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




    }
}
