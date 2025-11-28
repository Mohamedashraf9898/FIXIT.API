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
        Task<ReturnedOfferDto> SelectCraftsmanAsync(ClientSelectCraftsmanDto dto);
        Task<ReturnedOfferDto> ClientRespondToOfferAsync(ClientRespondDto dto);
        Task<ReturnedOfferDto> CraftsmanAcceptRequestAsync(CraftsmanAcceptDto dto);
        Task<ReturnedOfferDto> CraftsmanRejectRequestAsync(CraftsmanRejectDto dto);
        Task<ReturnedOfferDto> CraftsmanNewOfferAsync(CraftsManNewOfferDto dto);
        Task<bool> UpdateTotalAmountAsync(int serviceRequestId, decimal finalAmount);

    }
}
