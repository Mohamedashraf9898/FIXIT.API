using FIXIT.BLL.DTOs.SchedulingDTOs;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        
        [HttpGet("craftsman/{craftsmanId}")]
        public async Task<IActionResult> GetCraftsmanAvailability(int craftsmanId)
        {
            var availabilities = await _availabilityService.GetCraftsmanAvailabilityAsync(craftsmanId);
            return Ok(new
            {
                success = true,
                data = availabilities,
                message = "Availabilities retrieved successfully"
            });
        }

        [HttpGet("craftsman/{craftsmanId}/day/{dayOfWeek}")]
        public async Task<IActionResult> GetByDay(int craftsmanId, int dayOfWeek)
        {
            var availability = await _availabilityService.GetByDayAsync(craftsmanId, (DayOfWeek)dayOfWeek);
            return Ok(new
            {
                success = true,
                data = availability,
                message = "Availability retrieved successfully"
            });
        }

   
        [HttpPost]
        public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityDto dto)
        {
            var availability = await _availabilityService.CreateAvailabilityAsync(dto);
            return CreatedAtAction(nameof(GetAvailabilityById), new { id = availability.Id }, new
            {
                success = true,
                data = availability,
                message = "Availability created successfully"
            });
        }

      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAvailabilityById(int id)
        {
            var availability = await _availabilityService.GetAvailabilityByIdAsync(id);
            return Ok(new
            {
                success = true,
                data = availability,
                message = "Availability retrieved successfully"
            });
        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] UpdateAvailabilityDto dto)
        {
            var availability = await _availabilityService.UpdateAvailabilityAsync(id, dto);
            return Ok(new
            {
                success = true,
                data = availability,
                message = "Availability updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            await _availabilityService.DeleteAvailabilityAsync(id);
            return Ok(new
            {
                success = true,
                message = "Availability deleted successfully"
            });
        }

       
        [HttpGet("check-availability/{craftsmanId}/{dayOfWeek}")]
        public async Task<IActionResult> CheckAvailabilityOnDay(int craftsmanId, int dayOfWeek)
        {
            var isAvailable = await _availabilityService.IsAvailableOnDayAsync(craftsmanId, (DayOfWeek)dayOfWeek);
            return Ok(new
            {
                success = true,
                data = new { isAvailable },
                message = "Availability check completed"
            });
        }
        [HttpGet("slots")]
        public async Task<IActionResult> GetTimeSlots([FromQuery] int craftsmanId, [FromQuery] DateTime date, [FromQuery] int duration = 60)
        {
                       
                var slots = await _availabilityService.GetDailyTimeSlotsAsync(craftsmanId, date, duration);

                return Ok(new
                {
                    success = true,
                    data = slots,
                    message = "Available slots retrieved successfully"
                });
        }
    }
}
