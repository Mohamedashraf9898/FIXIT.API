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
            await _offerService.SelectCraftsmanAsync(dto);
            return Ok();
        }

        [HttpPost("client-respond")]
        public async Task<ActionResult> ClientRespond([FromBody] ClientRespondDto dto)
        {
            await _offerService.ClientRespondToOfferAsync(dto);
            return Ok();
        }

        [HttpPost("craftsman-accept")]
        public async Task<ActionResult> CraftsmanAccept([FromBody] CraftsmanAcceptDto dto)
        {
            await _offerService.CraftsmanAcceptRequestAsync(dto);
            return Ok();
        }

        [HttpPost("craftsman-reject")]
        public async Task<ActionResult> CraftsmanReject([FromBody] CraftsmanRejectDto dto)
        {
            await _offerService.CraftsmanRejectRequestAsync(dto);
            return Ok();
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
    }
}
