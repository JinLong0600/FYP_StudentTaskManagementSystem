using Microsoft.AspNetCore.Mvc;

namespace StudentTaskManagement.Controllers
{
    public class UtilitiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
