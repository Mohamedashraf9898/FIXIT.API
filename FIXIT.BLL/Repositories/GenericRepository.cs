using FIXIT.BLL.Interfaces;
using FIXIT.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T :class
    {
        private  readonly FixItDbContext _dbContext;

        public GenericRepository(FixItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(T t)
        {
            _dbContext.Set<T>().Remove(t);
        }

        public  async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();

        }

        public async Task<T> GetAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T t)
        {
             await _dbContext.Set<T>().AddAsync(t);
        }

        public  int Save()
        {
           return _dbContext.SaveChanges();
        }

        public Task<T> UpdateAsync(T t)
        {
            throw new NotImplementedException();
        }
    }
}
