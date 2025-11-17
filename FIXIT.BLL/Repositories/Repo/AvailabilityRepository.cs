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
    public class AvailabilityRepository : GenericRepository<CraftsManAvailability>, IAvailabilityRepository
    {
        private readonly FixItDbContext _dbContext;

        public AvailabilityRepository(FixItDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CraftsManAvailability>> GetByCraftsmanIdAsync(int craftsmanId)
        {
            return await _dbContext.Set<CraftsManAvailability>()
                .AsNoTracking()
                .Where(a => a.CraftsManId == craftsmanId)
                .OrderBy(a => a.DayOfWeek)
                .ToListAsync();
        }

        public async Task<CraftsManAvailability> GetByDayOfWeekAsync(int craftsmanId, DayOfWeek dayOfWeek)
        {
            return await _dbContext.Set<CraftsManAvailability>()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.CraftsManId == craftsmanId && a.DayOfWeek == dayOfWeek);
        }

        public async Task<List<CraftsManAvailability>> GetAvailableDaysAsync(int craftsmanId)
        {
            return await _dbContext.Set<CraftsManAvailability>()
                .AsNoTracking()
                .Where(a => a.CraftsManId == craftsmanId && a.IsAvailable == true)
                .OrderBy(a => a.DayOfWeek)
                .ToListAsync();
        }

        public async Task<bool> IsAvailableOnDayAsync(int craftsmanId, DayOfWeek dayOfWeek)
        {
            var availability = await GetByDayOfWeekAsync(craftsmanId, dayOfWeek);
            return availability != null && availability.IsAvailable;
        }

        public async Task<bool> IsAvailableAtTimeAsync(int craftsmanId, DayOfWeek dayOfWeek, TimeSpan time)
        {
            var availability = await GetByDayOfWeekAsync(craftsmanId, dayOfWeek);
            
            if (availability == null || !availability.IsAvailable)
                return false;

            return time >= availability.StartTime && time <= availability.EndTime;
        }
    }
}
