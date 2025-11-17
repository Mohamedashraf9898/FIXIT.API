using FIXIT.BLL.DTOs.SchedulingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface IAvailabilityService
    {
        // Create
        Task<AvailabilityDto> CreateAvailabilityAsync(CreateAvailabilityDto dto);

        // Read
        Task<AvailabilityDto> GetAvailabilityByIdAsync(int id);
        Task<List<AvailabilityDto>> GetCraftsmanAvailabilityAsync(int craftsmanId);
        Task<AvailabilityDto> GetByDayAsync(int craftsmanId, DayOfWeek dayOfWeek);

        // Update
        Task<AvailabilityDto> UpdateAvailabilityAsync(int id, UpdateAvailabilityDto dto);

        // Delete
        Task<bool> DeleteAvailabilityAsync(int id);

        // Check
        Task<bool> IsAvailableOnDayAsync(int craftsmanId, DayOfWeek dayOfWeek);
        Task<bool> IsAvailableAtTimeAsync(int craftsmanId, DayOfWeek dayOfWeek, TimeSpan time);
        Task<List<TimeSlotDto>> GetDailyTimeSlotsAsync(int craftsmanId, DateTime date, int durationMinutes);
    }
}
