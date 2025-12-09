using FIXIT.BLL.DTOs.ComplaintDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface IComplaintsService
    {
        Task<ResponseComplaintDto> AddComplaintAsync(CreateComplaintDto dto);
        Task<List<ResponseComplaintDto>> GetByServiceRequestIdAsync(int serviceRequestId);
        Task<List<ResponseComplaintDto>> GetAllAsync();
        Task<ResponseComplaintDto> RespondToComplaintAsync(RespondToComplaintDto dto);
    }
}
