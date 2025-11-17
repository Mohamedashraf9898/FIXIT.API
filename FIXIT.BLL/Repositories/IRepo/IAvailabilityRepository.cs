using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface IAvailabilityRepository : IGenericRepository<CraftsManAvailability>
    {
        // Get all availability slots for a craftsman
        Task<List<CraftsManAvailability>> GetByCraftsmanIdAsync(int craftsmanId);

        // Get availability for a specific day of week
        Task<CraftsManAvailability> GetByDayOfWeekAsync(int craftsmanId, DayOfWeek dayOfWeek);

        // Get all available days (not marked as unavailable)
        Task<List<CraftsManAvailability>> GetAvailableDaysAsync(int craftsmanId);

        // Check if craftsman is available on specific day
        Task<bool> IsAvailableOnDayAsync(int craftsmanId, DayOfWeek dayOfWeek);

        // Check if craftsman is available at specific time
        Task<bool> IsAvailableAtTimeAsync(int craftsmanId, DayOfWeek dayOfWeek, TimeSpan time);
    }
}
