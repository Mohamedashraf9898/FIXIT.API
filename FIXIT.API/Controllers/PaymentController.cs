using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.Services.IService.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController : ControllerBase
    {
        const string endpointSecret = "whsec_luY6et6407JQFPjTq1iz31bxLNLN71UF";
        public PaymentController(IPaymentService paymentService)
        {
            PaymentService = paymentService;
        }

        public IPaymentService PaymentService { get; }

        [HttpPost("{serviceRequestId}")]
        [Authorize]
        public async Task<ActionResult<ReadServiceRequestDto>> CreatePaymentIntent(int serviceRequestId)
        {

            var result = await PaymentService.CreateOrUpdatePaymentIntent(serviceRequestId);

            return Ok(result);


        }

        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            await PaymentService.UpdatePaymentStatus(json, HttpContext.Request.Headers["Stripe-Signature"]);

            return Ok();
        }
    }
}