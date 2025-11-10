using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ServicsDTOs;
using FIXIT.BLL.Exceptions;
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
			if (service == null)
				throw new ValidationException("Service data cannot be null.");

			await repo.AddAsync(mapper.Map<Service>(service));
			repo.Save();
		}

		public async Task<IEnumerable<GetAllServicesDTO>> GetAllServicesAsync()
		{
			var services = await repo.GetAllAsync();
			if (services == null || !services.Any())
				throw new NotFoundException(nameof(Service), "No services found.");

			return mapper.Map<List<GetAllServicesDTO>>(services);
		}

		public async Task<ServiceDto> GetServiceByNameAsync(string name)
		{
			var services = await repo.GetAllAsync();
			var service = services.FirstOrDefault(s => string.Equals(s.ServiceName, name, System.StringComparison.OrdinalIgnoreCase));

			if (service == null)
				throw new NotFoundException(nameof(Service), name);

			return mapper.Map<ServiceDto>(service);
		}

		public async Task<ServiceDto> GetServiceByIdAsync(int id)
		{
			var service = await repo.GetAsync(id);
			if (service == null)
				throw new NotFoundException(nameof(Service), id);

			return mapper.Map<ServiceDto>(service);
		}

		public void DeleteService(int id)
		{
			var service = repo.GetAsync(id).Result;
			if (service == null)
				throw new NotFoundException(nameof(Service), id);

			repo.Delete(id);
			repo.Save();
		}

		public bool UpdateService(int id, UpdateServiceDto updatedService)
		{
			if (id != updatedService.ServiceId)
				throw new ValidationException("ID mismatch between route and body.");

			var updated = repo.Update(mapper.Map<Service>(updatedService), id);
			if (!updated)
				throw new NotFoundException(nameof(Service), id);

			repo.Save();
			return true;
		}
	}
}
