using AutoMapper;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ServicsDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL
{
    public class ServiceServices : IServiceService
    {
        //not for requests or order service FOCUS
        private readonly IGenericRepository<Service> repo;
        private readonly IMapper mapper;

        public ServiceServices(IGenericRepository<Service> repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        //FIXIT.DAL.Models.Service
        public async Task CreateServiceAsync(CreateServiceDto service)
        {
            await repo.AddAsync(mapper.Map<DAL.Models.Service>(service));
            repo.Save();
        }

        public async Task<IEnumerable<GetAllServicesDTO>> GetAllServicesAsync()
        {
            var services = await repo.GetAllAsync();
            var result = mapper.Map<List<GetAllServicesDTO>>(services);
            return result;
        }
        public async Task<ServiceDto> GetServiceByNameAsync(string name)
        {
            var ServiceNames = await repo.GetAllAsync();
            var ServiceName = ServiceNames.FirstOrDefault
                                            (s => string.Equals(s.ServiceName, name, StringComparison.OrdinalIgnoreCase));
            if (ServiceName == null)
                return new ServiceDto { Message = "هذه الخدمة غير متاحة حاليًا." };

            return mapper.Map<ServiceDto>(ServiceName);
        }
        public async Task<ServiceDto> GetServiceByIdAsync(int id)
        {
            var service = await repo.GetAsync(id);
            if (service == null)
                return new ServiceDto { Message = "هذه الخدمة غير متاحة حاليًا." };

            return mapper.Map<ServiceDto>(service);
        }
        public void DeleteService(int id)
        {
            repo.Delete(id);
            repo.Save();
        }

        public bool UpdateService(int id, UpdateServiceDto UpdatedService)
        {
            if (repo.Update(mapper.Map<Service>(UpdatedService), id))
            {
                repo.Save();
                return true;

            }
            else
                return false;
        }
       


    }
}
