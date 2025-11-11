using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;

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

        #region ForPaymentService
        //osama
        Task<bool> CompleteServiceRequestAsync(int requestId);
        //end osama 
        #endregion

    }
}
