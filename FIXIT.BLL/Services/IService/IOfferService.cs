using FIXIT.BLL.DTOs.OfferDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface IOfferService
    {
        Task<bool> SelectCraftsmanAsync(ClientSelectCraftsmanDto dto);
        Task<bool> ClientRespondToOfferAsync(ClientRespondDto dto);
        Task<bool> CraftsmanAcceptRequestAsync(CraftsmanAcceptDto dto);
        Task<bool> CraftsmanRejectRequestAsync(CraftsmanRejectDto dto);
        Task<bool> CraftsmanNewOfferAsync(CraftsManNewOfferDto dto);
        Task<bool> UpdateTotalAmountAsync(int serviceRequestId, decimal finalAmount);

    }
}
