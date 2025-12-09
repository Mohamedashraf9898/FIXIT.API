using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.Repo
{
    public class ComplaintsRepository : IComplaintsRepository
    {
        private readonly FixItDbContext _dbContext;

        public ComplaintsRepository(FixItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddComplaintAsync(Complaint complaint)
        {
            await _dbContext.Complaints.AddAsync(complaint);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Complaint>> GetByServiceRequestIdAsync(int serviceRequestId)
        {
            return await _dbContext.Complaints
                .AsNoTracking()
                .Where(c => c.ServiceRequestId == serviceRequestId)
                .ToListAsync();
        }

        public async Task<List<Complaint>> GetAllAsync()
        {
            return await _dbContext.Complaints
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
