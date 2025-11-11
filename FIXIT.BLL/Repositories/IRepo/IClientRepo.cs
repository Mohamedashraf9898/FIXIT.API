using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface IClientRepo:IGenericRepository<Client>
    {
        public Task<Client> GetClientByEmailAsync(string email);

    }
}
