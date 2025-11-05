using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
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
        //osama
        Task<bool> CompleteServiceRequestAsync(int requestId);
        //end osama
        Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestForCraftsMan(string CraftsManName);
        Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestForClient(string ClientName);

    }
}
