using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface IWithdrawalRepository : IGenericRepository<WithdrawalRequest>
    {
        Task<List<WithdrawalRequest>> GetByCraftsManIdAsync(int craftsManId);
        Task<WithdrawalRequest?> GetPendingByIdAsync(int id);
    }

}
