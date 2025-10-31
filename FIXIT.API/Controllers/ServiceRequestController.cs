using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestController : ControllerBase
    {
        IServiceRequestService _serviceRequestService;
        public ServiceRequestController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllServiceRequests()
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceRequestService.GetAllServiceRequestAsync();

                if (result == null || !result.Any())
                    return NotFound("No service requests found.");

                return Ok(result);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRequestById(int id)
        {
          
            if (ModelState.IsValid)
            {
                var result = await _serviceRequestService.GetServiceRequestByIdAsync(id);
                if (result == null)
                    return NotFound($"Service request with ID {id} not found.");
                return Ok(result);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest( CreateServiceRequestDto serviceRequestDto)
        {
         
            if (ModelState.IsValid)
            {
                await _serviceRequestService.CreateServiceRequestAsync(serviceRequestDto);
                return Ok("Service request created successfully.");
            }
            else
                return BadRequest(ModelState);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
           
            if (ModelState.IsValid)
            {
                var result = await _serviceRequestService.DeleteServiceRequest(id);
                if (!result)
                    return NotFound($"Service request with ID {id} not found.");
                return Ok("Service request deleted successfully.");
            }
            else
                return BadRequest(ModelState);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(int id, UpdateServiceRequestDto serviceRequestDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceRequestService.UpdateServiceRequest(id, serviceRequestDto);
                if (!result)
                    return NotFound($"Service request with ID {id} not found.");
                return Ok("Service request updated successfully.");
            }
            else
                return BadRequest(ModelState); 
        }
        [HttpGet("Craftsman/{craftsManName}")]
        public async Task<IActionResult> GetAllServiceRequestForCraftsMan(string craftsManName)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceRequestService.GetAllServiceRequestForCraftsMan(craftsManName);

                if (result == null || !result.Any())
                    return NotFound($"No service requests found for craftsman '{craftsManName}'.");

                return Ok(result);
            }
            else
                return BadRequest(ModelState);
        }


        [HttpGet("Client/{clientName}")]
        public async Task<IActionResult> GetAllServiceRequestForClient(string clientName)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceRequestService.GetAllServiceRequestForClient(clientName);

                if (result == null || !result.Any())
                    return NotFound($"No service requests found for client '{clientName}'.");

                return Ok(result);
            }
            else
                return BadRequest(ModelState);
        }

    }
}
