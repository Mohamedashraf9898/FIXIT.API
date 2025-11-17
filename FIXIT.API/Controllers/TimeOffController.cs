using FIXIT.BLL.DTOs.SchedulingDTOs;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeOffController : ControllerBase
    {
        private readonly ITimeOffService _timeOffService;

        public TimeOffController(ITimeOffService timeOffService)
        {
            _timeOffService = timeOffService;
        }


        [HttpGet("craftsman/{craftsmanId}")]
        public async Task<IActionResult> GetCraftsmanTimeOffs(int craftsmanId)
        {
            var timeOffs = await _timeOffService.GetCraftsmanTimeOffsAsync(craftsmanId);
            return Ok(new
            {
                success = true,
                data = timeOffs,
                message = "Time offs retrieved successfully"
            });
        }

      
        [HttpGet("craftsman/{craftsmanId}/active")]
        public async Task<IActionResult> GetActiveTimeOffs(int craftsmanId)
        {
            var timeOffs = await _timeOffService.GetActiveTimeOffsAsync(craftsmanId);
            return Ok(new
            {
                success = true,
                data = timeOffs,
                message = "Active time offs retrieved successfully"
            });
        }

     
        [HttpGet("craftsman/{craftsmanId}/upcoming")]
        public async Task<IActionResult> GetUpcomingTimeOffs(int craftsmanId, [FromQuery] int days = 30)
        {
            var timeOffs = await _timeOffService.GetUpcomingTimeOffsAsync(craftsmanId, days);
            return Ok(new
            {
                success = true,
                data = timeOffs,
                message = "Upcoming time offs retrieved successfully"
            });
        }

 
        [HttpPost]
        public async Task<IActionResult> CreateTimeOff([FromBody] CreateTimeOffDto dto)
        {
            var timeOff = await _timeOffService.CreateTimeOffAsync(dto);
            return CreatedAtAction(nameof(GetTimeOffById), new { id = timeOff.Id }, new
            {
                success = true,
                data = timeOff,
                message = "Time off created successfully"
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimeOffById(int id)
        {
            var timeOff = await _timeOffService.GetTimeOffByIdAsync(id);
            return Ok(new
            {
                success = true,
                data = timeOff,
                message = "Time off retrieved successfully"
            });
        }

    
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeOff(int id, [FromBody] CreateTimeOffDto dto)
        {
            var timeOff = await _timeOffService.UpdateTimeOffAsync(id, dto);
            return Ok(new
            {
                success = true,
                data = timeOff,
                message = "Time off updated successfully"
            });
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeOff(int id)
        {
            await _timeOffService.DeleteTimeOffAsync(id);
            return Ok(new
            {
                success = true,
                message = "Time off deleted successfully"
            });
        }

        [HttpGet("check/{craftsmanId}")]
        public async Task<IActionResult> CheckTimeOffOnDate(int craftsmanId, [FromQuery] string date)
        {
            if (!System.DateTime.TryParse(date, out var parsedDate))
                return BadRequest(new { success = false, message = "Invalid date format" });

            var hasTimeOff = await _timeOffService.HasTimeOffOnDateAsync(craftsmanId, parsedDate);
            return Ok(new
            {
                success = true,
                data = new { hasTimeOff, date = parsedDate.ToString("yyyy-MM-dd") },
                message = "Time off check completed"
            });
        }
    }
}
