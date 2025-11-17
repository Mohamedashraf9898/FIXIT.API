using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.Service.Payment;
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
    public class ServiceRequestRepository : GenericRepository<ServicesRequest>, IServiceRequestRepository
    {
        private readonly FixItDbContext _dbContext;

        public ServiceRequestRepository(FixItDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServicesRequest> GetByIntentId(string intent_id)
        {
            var isexist =  await _dbContext.ServicesRequests.FirstOrDefaultAsync(sr => intent_id == sr.PaymentIntentId);

            return isexist!;
        }
        public async Task<List<ServicesRequest>> GetByDateAsync(int craftsmanId, DateTime date)
        {
            return await _dbContext.ServicesRequests
                .AsNoTracking()
                .Where(r => r.CraftsManId == craftsmanId
                         && r.ServiceStartTime.Date == date.Date
                         && r.Status != ServiceRequestStatus.RejectedByCraftsman
                         && r.Status != ServiceRequestStatus.Cancelled)
                .ToListAsync();
        }
    }
}
