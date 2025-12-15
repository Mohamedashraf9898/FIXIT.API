using FIXIT.BLL.DTOs.OfferDto;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Authorization;
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
                  var result =   await _serviceRequestService.CreateServiceRequestAsync(serviceRequestDto);
                return Ok(result);
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

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("StartAtTime/{id}")]
        public async Task<IActionResult> UpdateServiceRequestStartAtTime(int id, ConfirmStartatTimeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _serviceRequestService.UpdateServiceRequestStartAtTime(id, dto);
                if (!result)
                    return NotFound($"Service request with ID {id} not found.");
                return Ok("Service request start time updated successfully.");
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
        [HttpPost("cancel/{requestId}")]
        public async Task<IActionResult> CancelServiceRequest(int requestId, [FromBody] CancelServiceRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _serviceRequestService.CancelServiceRequestAsync(requestId, dto);

                if (!result)
                    return NotFound($"Service request with ID {requestId} not found.");

                return Ok("Service request cancelled successfully. Notifications sent to craftsman and admin.");
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
        [HttpGet("statuses")]
        public IActionResult GetStatuses()
        {
            var statuses = Enum.GetNames(typeof(ServiceRequestStatus));
            return Ok(statuses);
        }
        [Authorize(Roles ="Admin")] 
        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(ServiceRequestStatus status)
        {
            var result = await _serviceRequestService.GetRequestsByStatusAsync(status);
            return Ok(result);
        }
        [Authorize(Roles = "Admin,Client")]
        [HttpGet("by-client-status")]
        public async Task<IActionResult> GetByClientAndStatus(int clientId, ServiceRequestStatus status)
        {
            var result = await _serviceRequestService.GetRequestsByClientAndStatusAsync(clientId, status);
            return Ok(result);
        }
        [Authorize(Roles = "Admin,CraftsMan")]
        [HttpGet("by-craftsMan-status")]
        public async Task<IActionResult> GetByCraftsManAndStatus(int craftsManId, ServiceRequestStatus status)
        {
            var result = await _serviceRequestService.GetRequestsByCraftsmanAndStatusAsync(craftsManId, status);
            return Ok(result);
        }
        [HttpPost("report-issue/{requestId}")]
        public async Task<IActionResult> ReportIssue(int requestId, [FromBody] ReportIssueDto dto)
        {
             if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _serviceRequestService.ReportIssueAsync(requestId, dto);
                if (!result)
                    return NotFound("Service request not found or failed to report issue.");
                
                return Ok(new { message = "Issue reported successfully to admin." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
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

