/*using Microsoft.AspNetCore.Mvc;
using WebPush;
using System.Text.Json;

namespace StudentTaskManagement.Controllers
{
    public class PushNotificationController : Controller
    {
        private readonly IPushNotificationService _notificationService;

        public PushNotificationController(IPushNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] PushSubscription subscription)
        {
            await _notificationService.SaveSubscription(subscription);
            return Ok();
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationMessage message)
        {
            await _notificationService.SendNotification(message.Message);
            return Ok();
        }
    }
}
*/