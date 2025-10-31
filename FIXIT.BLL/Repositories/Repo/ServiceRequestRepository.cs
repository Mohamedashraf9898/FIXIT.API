using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL;
using FIXIT.DAL.Models;
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
    }
}
