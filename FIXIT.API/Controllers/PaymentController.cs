using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.Services.IService.Payment;
using FIXIT.BLL.Services.Service.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            PaymentService = paymentService;
            this._logger = logger;
        }

        public IPaymentService PaymentService { get; }

        [HttpPost("{serviceRequestId}")]
      //  [Authorize]
        public async Task<ActionResult<ReadServiceRequestDto>> CreatePaymentIntent(int serviceRequestId)
        {

            var result = await PaymentService.CreateOrUpdatePaymentIntent(serviceRequestId);

            return Ok(result);


        }
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> WebHook()
        {
            string json = null;

            try
            {
                _logger.LogInformation(" Webhook called at {Time}", DateTime.UtcNow);

               
                if (json == null)
                {
                    using (var reader = new StreamReader(Request.Body))
                    {
                        json = await reader.ReadToEndAsync();
                    }
                }

                if (string.IsNullOrEmpty(json))
                {
                    _logger.LogWarning(" Empty body");
                    return BadRequest("Empty request body");
                }

                _logger.LogInformation(" Received {Length} bytes", json.Length);

                var signature = Request.Headers["Stripe-Signature"].ToString();

                if (string.IsNullOrEmpty(signature))
                {
                    _logger.LogWarning(" Missing signature");
                    return BadRequest("Missing Stripe-Signature");
                }

                _logger.LogInformation(" Processing webhook synchronously...");

                await PaymentService.UpdatePaymentStatus(json, signature);

                _logger.LogInformation(" Webhook processed successfully");
                return Ok();
            }
            catch (StripeException )
            {
                //_logger.LogError(ex, " Stripe exception");
               
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Webhook endpoint exception");
                return Ok();
            }
        }
    }
}
    
