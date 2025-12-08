using FIXIT.BLL.DTOs.TimeSlotDTOs;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        public TimeSlotsController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        [HttpPost("generate/{craftsmanId}")]
        public async Task<IActionResult> GenerateSlots(int craftsmanId)
        {
            try
            {
                await _timeSlotService.GenerateSlotsForCraftsmanAsync(craftsmanId, 30);
                return Ok(new { message = "Time slots generated successfully for the next 30 days." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int craftsmanId, [FromQuery] DateTime date)
        {
            var slots = await _timeSlotService.GetAvailableSlotsAsync(craftsmanId, date);

            var response = slots.Select(s => new TimeSlotResponseDto
            {
                Id = s.Id,
                Date = s.Date.ToString("yyyy-MM-dd"),
                Time = DateTime.Today.Add(s.StartTime).ToString("hh:mm tt"),
                Status = s.Status.ToString(),
                PriceMultiplier = s.PriceMultiplier ?? 1.0m
            });

            return Ok(response);
        }
        [HttpGet("schedule")]
        public async Task<IActionResult> GetCraftsmanSchedule([FromQuery] int craftsmanId, [FromQuery] DateTime date)
        {
            var slots = await _timeSlotService.GetCraftsmanScheduleAsync(craftsmanId, date);

            var response = slots.Select(s => new TimeSlotResponseDto
            {
                Id = s.Id,
                Date = s.Date.ToString("yyyy-MM-dd"),
                Time = DateTime.Today.Add(s.StartTime).ToString("hh:mm tt"),
                Status = s.Status.ToString(),
                PriceMultiplier = s.PriceMultiplier ?? 1.0m
            });

            return Ok(response);
        }

        [HttpPut("toggle/{slotId}")]
        public async Task<IActionResult> ToggleSlot(int slotId, [FromQuery] int craftsmanId)
        {
            await _timeSlotService.ToggleSlotStatusAsync(slotId, craftsmanId);
            return Ok(new { message = "Slot status updated successfully" });
        }
    }
}
