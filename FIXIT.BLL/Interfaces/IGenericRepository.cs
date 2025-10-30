using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Interfaces
{
    public interface IGenericRepository<T> where T :class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(int Id);
        Task AddAsync(T t);
        bool Update(T t,int id);
        void Delete(int id);

        int Save();
    }
}
