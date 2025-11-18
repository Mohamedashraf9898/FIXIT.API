using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService.Payment
{
    public interface IVodafoneCashService
    {
        Task<bool> SendMoneyAsync(string phoneNumber, decimal amount);
        Task<bool> TransferAsync(string phoneNumber, decimal amount);
    }

}
