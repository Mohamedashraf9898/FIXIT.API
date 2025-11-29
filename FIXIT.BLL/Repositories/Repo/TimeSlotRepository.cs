using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.Repo
{
    public class TimeSlotRepository : GenericRepository<TimeSlot>, ITimeSlotRepository
    {
        private readonly FixItDbContext _context;

        public TimeSlotRepository(FixItDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int craftsmanId, DateTime date)
        {
            return await _context.TimeSlots
                .AnyAsync(ts => ts.CraftsManId == craftsmanId && ts.Date.Date == date.Date);
        }

        public async Task<List<TimeSlot>> GetAvailableSlotsAsync(int craftsmanId, DateTime date)
        {
            return await _context.TimeSlots
                .AsNoTracking()
                .Where(ts => ts.CraftsManId == craftsmanId
                          && ts.Date.Date == date.Date
                          //&& ts.Status == SlotStatus.Available
                          )
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();
        }
        public async Task<TimeSlot> GetSlotByDateAndTimeAsync(int craftsmanId, DateTime serviceStartTime)
        {
            return await _context.TimeSlots
                .AsNoTracking() 
                .FirstOrDefaultAsync(ts =>
                    ts.CraftsManId == craftsmanId &&
                    ts.Date.Date == serviceStartTime.Date &&
                    ts.StartTime.Hours == serviceStartTime.Hour &&
                    ts.StartTime.Minutes == serviceStartTime.Minute);
        }
        public async Task AddRangeAsync(List<TimeSlot> slots)
        {
            await _context.TimeSlots.AddRangeAsync(slots);
        }
        public async Task<TimeSlot> GetSlotByRequestIdAsync(int serviceRequestId)
        {
            return await _context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.ServiceRequestId == serviceRequestId);
        }
    }
}
