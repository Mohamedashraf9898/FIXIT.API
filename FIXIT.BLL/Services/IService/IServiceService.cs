using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ServicsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface IServiceService
    {
        //1.CRUD + GetByName , GetBy Location + GetBY Price
        Task<ServiceDto> GetServiceByNameAsync(string name);

        Task<IEnumerable<GetAllServicesDTO>> GetAllServicesAsync();
        Task<ServiceDto> GetServiceByIdAsync(int id);
        Task CreateServiceAsync(CreateServiceDto service);

        bool UpdateService(int id, UpdateServiceDto Service);
        void DeleteService(int id);

        //Task<IEnumerable<CraftsmanDto>> GetCraftsmenByServiceNearbyAsync(int serviceId);

        //Task<IEnumerable<CraftsManDto>> GetCraftsmenByServiceSortedByPriceAsync(int serviceId);
    }
}
