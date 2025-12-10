using FIXIT.BLL.DTOs.ComplaintDtos;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FIXIT.API.Controllers
{
    [ApiController]
    [Route("api/complaints")]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintsService _complaintsService;

        public ComplaintsController(IComplaintsService complaintsService)
        {
            _complaintsService = complaintsService;
        }

        // Keep existing generic create (optional)
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateComplaintDto dto)
        {
            var result = await _complaintsService.AddComplaintAsync(dto);
            return Ok(result);
        }

        // NEW: client-specific create endpoint
        [HttpPost("client/create")]
        public async Task<IActionResult> CreateByClient([FromBody] CreateComplaintDto dto)
        {
            // ensure only client flow: clear CraftsManId
            dto.CraftsManId = null;
            // dto.ClientId must be provided by client (or if you have auth, override with user id)
            if (!dto.ClientId.HasValue)
                return BadRequest("ClientId must be provided for client complaints.");

            var result = await _complaintsService.AddComplaintAsync(dto);
            return Ok(result);
        }

        // NEW: craftsman-specific create endpoint
        [HttpPost("craftsman/create")]
        public async Task<IActionResult> CreateByCraftsman([FromBody] CreateComplaintDto dto)
        {
            // ensure only craftsman flow: clear ClientId
            dto.ClientId = null;
            if (!dto.CraftsManId.HasValue)
                return BadRequest("CraftsManId must be provided for craftsman complaints.");

            var result = await _complaintsService.AddComplaintAsync(dto);
            return Ok(result);
        }

        [HttpGet("byServiceRequest/{id}")]
        public async Task<IActionResult> GetByServiceRequest(int id)
        {
            var result = await _complaintsService.GetByServiceRequestIdAsync(id);
            return Ok(result);
        }

        // NEW: get complaints for a client for a specific service request
        [HttpGet("client/{clientId}/service-request/{serviceRequestId}")]
        public async Task<IActionResult> GetForClient(int clientId, int serviceRequestId)
        {
            var result = await _complaintsService.GetForClientAsync(clientId, serviceRequestId);
            return Ok(result);
        }

        // NEW: get complaints for a craftsman for a specific service request
        [HttpGet("craftsman/{craftsManId}/service-request/{serviceRequestId}")]
        public async Task<IActionResult> GetForCraftsman(int craftsManId, int serviceRequestId)
        {
            var result = await _complaintsService.GetForCraftsmanAsync(craftsManId, serviceRequestId);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _complaintsService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("respond")]
        public async Task<IActionResult> Respond([FromBody] RespondToComplaintDto dto)
        {
            var result = await _complaintsService.RespondToComplaintAsync(dto);
            return Ok(result);
        }
    }
}
