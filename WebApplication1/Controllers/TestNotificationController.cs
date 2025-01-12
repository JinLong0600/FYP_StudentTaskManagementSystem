using Microsoft.AspNetCore.Mvc;
using WebPush;


namespace StudentTaskManagement.Controllers
{
    // Create a test controller for generating keys
    [ApiController]
    [Route("api/[controller]")]
    public class TestNotificationController : Controller
    {
        [HttpGet("generate-vapid")]
        public IActionResult GenerateVapidKeys()
        {
            var vapidKeys = VapidHelper.GenerateVapidKeys();
            return Ok(new
            {
                PublicKey = vapidKeys.PublicKey,
                PrivateKey = vapidKeys.PrivateKey
            });
        }

    }
}


