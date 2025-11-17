using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface ITimeOffRepository : IGenericRepository<CraftsManTimeOff>
    {
        // Get all time off for a craftsman
        Task<List<CraftsManTimeOff>> GetByCraftsmanIdAsync(int craftsmanId);

        // Get current/active time offs (between start and end date)
        Task<List<CraftsManTimeOff>> GetActiveDaysAsync(int craftsmanId);

        // Get time off for a specific date
        Task<CraftsManTimeOff> GetByDateAsync(int craftsmanId, DateTime date);

        // Check if craftsman has time off on specific date
        Task<bool> HasTimeOffOnDateAsync(int craftsmanId, DateTime date);

        // Get time offs by type
        Task<List<CraftsManTimeOff>> GetByTypeAsync(int craftsmanId, TimeOffType type);

        // Get upcoming time offs
        Task<List<CraftsManTimeOff>> GetUpcomingAsync(int craftsmanId, int days = 30);
    }
}
