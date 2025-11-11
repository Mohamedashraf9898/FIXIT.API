using FIXIT.BLL.DTOs.ServicsDTOs;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServiceController(IServiceService serviceService)
        {
            _service = serviceService;
        }
		[HttpGet]
		public async Task<IActionResult> GetAllServices()
		{
			var services = await _service.GetAllServicesAsync();
			return Ok(services);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetServiceById(int id)
		{
			var service = await _service.GetServiceByIdAsync(id);
			return Ok(service);
		}

		[HttpPost]
		public async Task<IActionResult> AddService([FromBody] CreateServiceDto serviceDto)
		{
			await _service.CreateServiceAsync(serviceDto);
			return StatusCode(StatusCodes.Status201Created);
		}

		[HttpPut("{id}")]
		public IActionResult UpdateService(int id, [FromBody] UpdateServiceDto serviceDto)
		{
			_service.UpdateService(id, serviceDto);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteService(int id)
		{
			_service.DeleteService(id);
			return NoContent();
		}

	}
}
