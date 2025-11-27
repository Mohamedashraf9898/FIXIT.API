using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;
        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetClientNotifications(int clientId)
        {
            var result = await _service.GetNotificationsForClientAsync(clientId);
            return Ok(result);
        }

        [HttpGet("craftsman/{craftsManId}")]
        public async Task<IActionResult> GetCraftsmanNotifications(int craftsManId)
        {
            var result = await _service.GetNotificationsForCraftsmanAsync(craftsManId);
            return Ok(result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _service.MarkAsReadAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
