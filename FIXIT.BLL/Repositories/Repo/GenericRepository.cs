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
    public class GenericRepository<T> : IGenericRepository<T> where T :class
    {
        private  readonly FixItDbContext _dbContext;

        public GenericRepository(FixItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Set<T>().Find(id);
            if (entity != null)
            {
                _dbContext.Set<T>().Remove(entity);
            }
        }

        public  async Task<List<T>> GetAllAsync()
        {
			if (typeof(T) == typeof(CraftsMan))
			{
				var craftsmen = await _dbContext.CraftsMan
					.Where(e => e.IsVerified == true)
					.AsNoTracking()
					.ToListAsync();

				return craftsmen.Cast<T>().ToList();
			}
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

        public bool Update(T t,int id )
        {
            var res = _dbContext.Set<T>().Find(id);
            if (res == null)
                return false;
            else
            {
                _dbContext.Entry(res).CurrentValues.SetValues(t);
                return true;
            }
        }
    }
}
