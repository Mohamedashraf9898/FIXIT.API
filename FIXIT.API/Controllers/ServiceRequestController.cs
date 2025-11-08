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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _serviceRequestService.DeleteServiceRequest(id);

                if (!result)
                    return NotFound($"Service request with ID {id} not found.");

                return Ok("Service request deleted successfully.");
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(new { message = ex.Message }); 
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(int id, UpdateServiceRequestDto serviceRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _serviceRequestService.UpdateServiceRequest(id, serviceRequestDto);

                if (!result)
                    return NotFound($"Service request with ID {id} not found.");

                return Ok("Service request updated successfully.");
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


        [HttpGet("Craftsman/ById/{craftsManId}")]
        public async Task<IActionResult> GetAllServiceRequestsForCraftsManById(int craftsManId)
        {
            try
            {
                var result = await _serviceRequestService.GetAllServiceRequestsForCraftsManById(craftsManId);

                if (!result.Any())
                    return NotFound($"No service requests found for craftsman ID {craftsManId}.");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Client/ById/{clientId}")]
        public async Task<IActionResult> GetAllServiceRequestsForClientById(int clientId)
        {
            try
            {
                var result = await _serviceRequestService.GetAllServiceRequestsForClientById(clientId);

                if (!result.Any())
                    return NotFound($"No service requests found for client ID {clientId}.");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{serviceRequestId}/Craftsmen")]
        public async Task<IActionResult> GetCraftsmenForServiceRequest(int serviceRequestId)
        {
            var craftsmen = await _serviceRequestService.GetCraftsmenByLocationAsync(serviceRequestId);

            if (craftsmen == null || !craftsmen.Any())
                return NotFound("No craftsmen found for this service request.");

            return Ok(craftsmen);
        }

        #region ForPaymentService
        [HttpPost("complete/{requestId}")]
        public async Task<IActionResult> CompleteServiceRequest(int requestId)
        {

            var result = await _serviceRequestService.CompleteServiceRequestAsync(requestId);
            if (!result)
                return BadRequest("Failed to complete service request or process transaction.");

            return Ok($"Service request #{requestId} completed successfully and craftsman's wallet updated.");
        } 
        #endregion
    }
}

