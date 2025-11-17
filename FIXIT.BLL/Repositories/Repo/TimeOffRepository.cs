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
    public class TimeOffRepository : GenericRepository<CraftsManTimeOff>, ITimeOffRepository
    {
        private readonly FixItDbContext _dbContext;

        public TimeOffRepository(FixItDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CraftsManTimeOff>> GetByCraftsmanIdAsync(int craftsmanId)
        {
            return await _dbContext.Set<CraftsManTimeOff>()
                .AsNoTracking()
                .Where(t => t.CraftsManId == craftsmanId)
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();
        }

        public async Task<List<CraftsManTimeOff>> GetActiveDaysAsync(int craftsmanId)
        {
            var now = DateTime.UtcNow;
            return await _dbContext.Set<CraftsManTimeOff>()
                .AsNoTracking()
                .Where(t => t.CraftsManId == craftsmanId && 
                           t.StartDate <= now && 
                           t.EndDate >= now)
                .ToListAsync();
        }

        public async Task<CraftsManTimeOff> GetByDateAsync(int craftsmanId, DateTime date)
        {
            return await _dbContext.Set<CraftsManTimeOff>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.CraftsManId == craftsmanId && 
                                         t.StartDate.Date <= date.Date && 
                                         t.EndDate.Date >= date.Date);
        }

        public async Task<bool> HasTimeOffOnDateAsync(int craftsmanId, DateTime date)
        {
            return await _dbContext.Set<CraftsManTimeOff>()
                .AsNoTracking()
                .AnyAsync(t => t.CraftsManId == craftsmanId && 
                              t.StartDate.Date <= date.Date && 
                              t.EndDate.Date >= date.Date);
        }

        public async Task<List<CraftsManTimeOff>> GetByTypeAsync(int craftsmanId, TimeOffType type)
        {
            return await _dbContext.Set<CraftsManTimeOff>()
                .AsNoTracking()
                .Where(t => t.CraftsManId == craftsmanId && t.Type == type)
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();
        }

        public async Task<List<CraftsManTimeOff>> GetUpcomingAsync(int craftsmanId, int days = 30)
        {
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            return await _dbContext.Set<CraftsManTimeOff>()
                .AsNoTracking()
                .Where(t => t.CraftsManId == craftsmanId && 
                           t.StartDate >= startDate && 
                           t.StartDate <= endDate)
                .OrderBy(t => t.StartDate)
                .ToListAsync();
        }
    }
}
