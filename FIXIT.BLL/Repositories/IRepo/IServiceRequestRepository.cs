using FIXIT.BLL.Repositories.Repo;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface IServiceRequestRepository  : IGenericRepository<ServicesRequest> 
    {
        public Task<ServicesRequest> GetByIntentId(string paymentIntentId);
    }
}
