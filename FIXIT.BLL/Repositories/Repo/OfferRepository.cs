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
    public class OfferRepository : IOfferRepository
    {
        private readonly FixItDbContext _dbContext;

        public OfferRepository(FixItDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Delete(int id)
        {
            var entity = _dbContext.Offers.Find(id);
            if (entity != null)
            {
                _dbContext.Offers.Remove(entity);
            }
        }

        public async Task<List<Offer>> GetAllAsync()
        {
                         
            return await _dbContext.Offers.AsNoTracking().ToListAsync();

        }

        public async Task<Offer> GetAsync(int id)
        {
            return await _dbContext.Offers.FindAsync(id);
        }

        public async Task AddAsync(Offer offer)
        {
            await _dbContext.Offers.AddAsync(offer);
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public bool Update(Offer offer, int id)
        {
            var res = _dbContext.Offers.Find(id);
            if (res == null)
                return false;
            else
            {
                _dbContext.Entry(res).CurrentValues.SetValues(offer);
                return true;
            }
        }
    }
}
