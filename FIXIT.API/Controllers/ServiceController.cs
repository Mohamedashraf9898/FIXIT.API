using FIXIT.BLL.DTOs.ServicsDTOs;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _Service;

        public ServiceController(IServiceService serviceService)
        {
            _Service = serviceService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllService()
        {
            var services = await _Service.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _Service.GetServiceByIdAsync(id);
            if (service == null)
                return NotFound();

            return Ok(service);
        }
        //ADD
        [HttpPost("AddService")]
        public async Task<IActionResult> AddService([FromBody] CreateServiceDto createdService)
        {
            if (createdService == null)
                return BadRequest("Service data is required");

            await _Service.CreateServiceAsync(createdService);
            return Ok("Service created successfully");
        }



        //[HttpGet("GetCraftsmenByServiceNearby")]
        //public async Task<IActionResult> GetCraftsmenByServiceNearby(int serviceId, string clientAddress)
        //{
        //    if (serviceId <= 0)
        //        return BadRequest("Invalid service ID");

        //    if (string.IsNullOrWhiteSpace(clientAddress))
        //        return BadRequest("Client address is required");

        //    try
        //    {
        //        var craftsmen = await server_server.GetCraftsmenByServiceNearbyAsync(serviceId, clientAddress);

        //        if (craftsmen == null || !craftsmen.Any())
        //            return NotFound("No craftsmen found nearby for this service.");

        //        return Ok(craftsmen);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}


        [HttpPut("{id}")]
        public IActionResult UpdateService(int id, UpdateServiceDto updatedService)
        {
            if (id != updatedService.ServiceId)
            {
                return BadRequest("ID in URL does not match ID in the request body.");
            }

            var success = _Service.UpdateService(id, updatedService);

            if (success)
            {
                return NoContent(); 
            }
            else
            {
                return NotFound($"Service with ID {id} not found."); 
            }
        }



        [HttpDelete("{id}")]
        public IActionResult DeleteService(int id)
        {
            _Service.DeleteService(id);
            return NoContent();
        }

    }
}
