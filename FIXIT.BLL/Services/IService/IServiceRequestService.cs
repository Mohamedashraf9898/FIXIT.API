using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.OfferDto;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface IServiceRequestService
    {
        Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestAsync();
        Task<ReadServiceRequestDto> GetServiceRequestByIdAsync(int id);
        Task<bool> CreateServiceRequestAsync(CreateServiceRequestDto ServiceRequestDto);
        Task<bool> UpdateServiceRequest(int id, UpdateServiceRequestDto ServiceRequestDto);
        Task<bool> DeleteServiceRequest(int id);
        Task<List<CraftsManDto>> GetCraftsmenByLocationAsync(int serviceRequestId);


        Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestsForCraftsManById(int craftsManId);
        Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestsForClientById(int clientId);
        #region For Offer
        Task<bool> SelectCraftsmanAsync(ClientSelectCraftsmanDto dto);
        Task<bool> ClientRespondToOfferAsync(ClientRespondDto dto);
        Task<bool> CraftsmanAcceptRequestAsync(CraftsmanAcceptDto dto);
        Task<bool> CraftsmanRejectRequestAsync(CraftsmanRejectDto dto);
        Task<bool> CraftsmanNewOfferAsync(CraftsManNewOfferDto dto);
        Task<bool> UpdateTotalAmountAsync(int serviceRequestId, decimal finalAmount);
       


        #endregion

        #region ForPaymentService
        //osama
        Task<bool> CompleteServiceRequestAsync(int requestId);
        //end osama 
        #endregion

    }
}
