using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface IOfferRepository
    {
        Task<List<Offer>> GetAllAsync();
        Task<Offer> GetAsync(int id);
        Task AddAsync(Offer offer);
        bool Update(Offer offer, int id);
        void Delete(int id);

        int Save();
    }
}
