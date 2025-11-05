using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface IReviewRepository : IGenericRepository<Review>
    {

        Task<bool> DoesReviewExistForRequestAsync(int serviceRequestId);
        Task<IEnumerable<Review>> GetReviewsForCraftsmanAsync(int craftsmanId);
    }

}
