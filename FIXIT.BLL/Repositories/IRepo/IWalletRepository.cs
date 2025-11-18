using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface IWalletRepository : IGenericRepository<Wallet>
    {
        Task<Wallet> GetWalletByCraftsManIdAsync(int craftsManId);
        Task AddTransactionAsync(WalletTransaction transaction);
        Task UpdateWalletAsync(Wallet wallet);
    }
}
