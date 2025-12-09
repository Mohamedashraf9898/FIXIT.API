using FIXIT.BLL.DTOs.ComplaintDtos;
using FIXIT.BLL.Services.IService;
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

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateComplaintDto dto)
        {
            var result = await _complaintsService.AddComplaintAsync(dto);
            return Ok(result);
        }

        [HttpGet("byServiceRequest/{id}")]
        public async Task<IActionResult> GetByServiceRequest(int id)
        {
            var result = await _complaintsService.GetByServiceRequestIdAsync(id);
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
