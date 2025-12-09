using FIXIT.BLL.DTOs.Contact;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FIXIT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public ContactController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] ContactFormDto contactForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _emailService.SendContactEmailAsync(contactForm);
                return Ok(new { message = "Your message has been sent successfully." });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("cancellation")]
        public async Task<IActionResult> SendCancellationRequest([FromBody] CancellationRequestDto cancellationRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Send email to admin
                await _emailService.SendContactEmailAsync(new ContactFormDto
                {
                    FullName = cancellationRequest.FullName,
                    Email = cancellationRequest.Email,
                    Phone = cancellationRequest.Phone,
                    Message = cancellationRequest.Message
                });

                // Send notifications (if you want to handle notifications separately)
                // await _notificationService.SendCancellationNotificationsAsync(cancellationRequest);

                return Ok(new { message = "Cancellation request sent successfully." });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
