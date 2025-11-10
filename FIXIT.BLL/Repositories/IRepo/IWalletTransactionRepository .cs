using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface IWalletTransactionRepository : IGenericRepository<WalletTransaction>
    {
        Task<WalletTransaction> GetByServiceRequestIdAsync(int serviceRequestId);
        Task<IEnumerable<WalletTransaction>> GetAllByWalletIdAsync(int walletId);
    }
}
