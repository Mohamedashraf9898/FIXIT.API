using FIXIT.BLL.DTOs.OfferDto;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FIXIT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpPost("select-craftsman")]
        public async Task<ActionResult> SelectCraftsman([FromBody] ClientSelectCraftsmanDto dto)
        {
            var result  =  await _offerService.SelectCraftsmanAsync(dto);
            return Ok(result);
        }

        [HttpPost("client-respond")]
        public async Task<ActionResult> ClientRespond([FromBody] ClientRespondDto dto)
        {
           var result = await _offerService.ClientRespondToOfferAsync(dto);
            return Ok(result);
        }

        [HttpPost("craftsman-accept")]
        public async Task<ActionResult> CraftsmanAccept([FromBody] CraftsmanAcceptDto dto)
        {
           var result =  await _offerService.CraftsmanAcceptRequestAsync(dto);
            return Ok(result);
        }

        [HttpPost("craftsman-reject")]
        public async Task<ActionResult> CraftsmanReject([FromBody] CraftsmanRejectDto dto)
        {
            var result = await _offerService.CraftsmanRejectRequestAsync(dto);
            return Ok(result);
        }

        [HttpPost("craftsman-new-offer")]
        public async Task<ActionResult> CraftsmanNewOffer([FromBody] CraftsManNewOfferDto dto)
        {
            var result = await _offerService.CraftsmanNewOfferAsync(dto);
            return Ok(result);
        }

        [HttpPut("{serviceRequestId}/update-total-amount")]
        public async Task<ActionResult> UpdateTotalAmount(int serviceRequestId, [FromBody] decimal finalAmount)
        {
            await _offerService.UpdateTotalAmountAsync(serviceRequestId, finalAmount);
            return Ok();
        }
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult> GetOfferById(int id)
        {
            var offer = await _offerService.GetOfferById(id);
            return Ok(offer);
        }

        [HttpPost("craftsman-apologize")]
        public async Task<ActionResult> CraftsmanApologize([FromBody] CraftsmanApologizeDto dto)
        {
            try
            {
                var result = await _offerService.CraftsmanApologizeAsync(dto);
                return Ok(new
                {
                    success = result,
                    message = "Apology processed. Client has been notified to choose Refund or New Craftsman."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }
    }
}
