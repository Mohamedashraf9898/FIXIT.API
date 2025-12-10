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
        //Task<bool> CreateServiceRequestAsync(CreateServiceRequestDto dto);
        Task<ReturnedServiceRequestDto> CreateServiceRequestAsync(CreateServiceRequestDto dto);
        Task<bool> UpdateServiceRequest(int id, UpdateServiceRequestDto dto);
        Task<bool> UpdateServiceRequestStartAtTime(int id, ConfirmStartatTimeDto dto);
        Task<bool> DeleteServiceRequest(int id);
        Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestsForCraftsManById(int craftsManId);
        Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestsForClientById(int clientId);

        Task<bool> CancelServiceRequestAsync(int serviceRequestId, CancelServiceRequestDto dto);
        Task<IEnumerable<ReadServiceRequestDto>> GetRequestsByStatusAsync(ServiceRequestStatus status);
        



            #region ForPaymentService
            //osama
            Task<bool> CompleteServiceRequestAsync(int serviceRequestId);

        //end osama 
        #endregion

    }
}
