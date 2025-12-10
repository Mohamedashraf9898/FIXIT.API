using FIXIT.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface IComplaintsRepository
    {
        Task AddComplaintAsync(Complaint complaint);
        Task<List<Complaint>> GetByServiceRequestIdAsync(int serviceRequestId);
        Task<List<Complaint>> GetAllAsync();
        Task<Complaint> GetByIdAsync(int id);
        Task UpdateComplaintAsync(Complaint complaint);
        Task<List<Complaint>> GetForClientAsync(int clientId, int serviceRequestId);
        Task<List<Complaint>> GetForCraftsmanAsync(int craftsmanId, int serviceRequestId);
    }
}
