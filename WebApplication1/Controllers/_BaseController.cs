using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StudentTaskManagement.Controllers
{
    public class _BaseController : Controller
    {
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
