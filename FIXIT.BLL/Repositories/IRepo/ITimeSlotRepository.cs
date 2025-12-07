using FIXIT.BLL.Repositories.Repo;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface ITimeSlotRepository : IGenericRepository<TimeSlot>
    {
        Task<bool> ExistsAsync(int craftsmanId, DateTime date);

        Task<List<TimeSlot>> GetAvailableSlotsAsync(int craftsmanId, DateTime date);

        Task AddRangeAsync(List<TimeSlot> slots);
        Task<TimeSlot> GetSlotByDateAndTimeAsync(int craftsmanId, DateTime serviceStartTime);
        Task<TimeSlot> GetSlotByRequestIdAsync(int serviceRequestId);
        Task<List<TimeSlot>> GetAllSlotsByDateAsync(int craftsmanId, DateTime date);

    }
}
