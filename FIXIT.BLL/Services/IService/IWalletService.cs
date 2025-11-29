using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.BLL.DTOs.WalletDTos;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Services.IService
{
    public interface IWalletService
    {
        Task<WalletDto> GetWalletAsync(int craftsManId);
        Task<bool> AddFundsAsync(CreateWalletTransactionDto dto);
        Task<bool> WithdrawFundsAsync(CreateWalletTransactionDto dto);
        Task<IEnumerable<WalletTransactionDto>> GetWalletTransactionsAsync(int craftsManId);
        Task<UpdateWaletTransactionDto> UpdateWaletTransaction(UpdateWaletTransactionDto dto);

        //Task<bool> DeductFundsAsync(int craftsManId, decimal amount, string description);
    }
}
