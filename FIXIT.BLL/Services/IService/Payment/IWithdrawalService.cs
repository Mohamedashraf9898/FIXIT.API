using FIXIT.BLL.DTOs.WithdrawalDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService.Payment
{
    public interface IWithdrawalService
    {
        Task<ReadWithdrawalDto> CreateWithdrawalAsync(int craftsManId, WithdrawalRequestDto dto);
        Task<List<ReadWithdrawalDto>> GetPendingWithdrawalsAsync();
        Task<ReadWithdrawalDto> ApproveAsync(int withdrawalId);
    }


}
