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
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly FixItDbContext _dbContext;
        public ReviewRepository(FixItDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DoesReviewExistForRequestAsync(int serviceRequestId)
        {
            return await _dbContext.Set<Review>()
                .AsNoTracking().AnyAsync(r => r.ServicesRequestId == serviceRequestId);
        }

        public async Task<IEnumerable<Review>> GetReviewsForCraftsmanAsync(int craftsmanId)
        {

            return await _dbContext.Set<Review>()
                .AsNoTracking()
                .Where(r => r.CraftsManId == craftsmanId)
                .ToListAsync();
        }

        public async Task<Review> GetReviewByServiceRequestIdAsync(int serviceRequestId)
        {
            return await _dbContext.Set<Review>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ServicesRequestId == serviceRequestId);
        }

        public async Task<double> GetAverageRatingForCraftsmanAsync(int craftsmanId)
        {
            return await _dbContext.Set<Review>()
                .Where(r => r.CraftsManId == craftsmanId)
                .AverageAsync(r => (double?)r.RatingValue) ?? 0.0;
        }
    }
}
