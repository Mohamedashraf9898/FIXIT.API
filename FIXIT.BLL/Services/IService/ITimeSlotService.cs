using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface ITimeSlotService
    {
        Task GenerateSlotsForCraftsmanAsync(int craftsmanId, int daysAhead = 30);
        Task<List<TimeSlot>> GetAvailableSlotsAsync(int craftsmanId, DateTime date);
    }
}
