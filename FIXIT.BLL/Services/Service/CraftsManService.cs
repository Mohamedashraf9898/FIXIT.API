
using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Repositories.Repo;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service
{
	public class CraftsManService : ICraftsManService
	{
		private readonly ICraftsManRepo craftsManRepo;
		private readonly IGenericRepository<CraftsManService> generic;
		private readonly IMapper mapper;

		public CraftsManService(ICraftsManRepo craftsManRepo,IGenericRepository<CraftsManService> generic,IMapper mapper) 
		{
			this.craftsManRepo = craftsManRepo;
			this.generic = generic;
			this.mapper = mapper;
		}
		public async Task<List<CraftsManDto>> GetAllCraftsMenAsync()
		{
			var craftsMen = await craftsManRepo.GetAllAsync();
			if (craftsMen == null || !craftsMen.Any())
				throw new NotFoundException(nameof(CraftsMan), "No craftsmen found.");

			return mapper.Map<List<CraftsManDto>>(craftsMen);
		}
		public async Task<CraftsManDto> GetCraftsManByIdAsync(int id)
		{
			var craftsMan = await craftsManRepo.GetAsync(id);
			if (craftsMan == null)
				throw new NotFoundException(nameof(CraftsMan), id);

			return mapper.Map<CraftsManDto>(craftsMan);
		}
		public async Task<List<CraftsManDto>> GetCraftsMenByNameAsync(string? fName, string? lName)
		{
			var craftsMen = await craftsManRepo.GetCraftsManByNameAsync(fName, lName);

			if (craftsMen == null || !craftsMen.Any())
				throw new NotFoundException(nameof(CraftsMan), "No craftsmen found with the given name.");

			return mapper.Map<List<CraftsManDto>>(craftsMen);
		}
		public async Task<List<CraftsManDto>> GetCraftsMenByLocationandServiceAsync(string location, string servicename)
		{
			if (string.IsNullOrWhiteSpace(location) || string.IsNullOrWhiteSpace(servicename))
				throw new ValidationException("Location and ServiceName are required.");

			var craftsmen = await craftsManRepo.GetCraftsMenByLocationandServiceAsync(location, servicename);

			if (craftsmen == null || !craftsmen.Any())
				throw new NotFoundException(nameof(CraftsMan), "No craftsmen found for this location and service.");

			return mapper.Map<List<CraftsManDto>>(craftsmen);
		}
		public async Task CreateCraftsManAsync(CreateCraftsManDto craftsManDto)
		{

			if (craftsManDto == null)
				throw new ValidationException("Craftsman data cannot be null.");

			await craftsManRepo.AddAsync(mapper.Map<CraftsMan>(craftsManDto));
			craftsManRepo.Save();
		}

		public void DeleteCraftsMan(int id)
		{
			var craftsMan = craftsManRepo.GetAsync(id).Result;
			if (craftsMan == null)
				throw new NotFoundException(nameof(CraftsMan), id);

			craftsManRepo.Delete(id);
			craftsManRepo.Save();
		}

		
		public bool UpdateCraftsMan(int id,UpdateCraftsManDto craftsManDto)
		{
			if (craftsManDto == null)
				throw new ValidationException("Craftsman data cannot be null.");

			if (id != craftsManDto.Id)
				throw new ValidationException("Id mismatch between route and body.");

			var updated = craftsManRepo.Update(mapper.Map<CraftsMan>(craftsManDto), id);
			if (!updated)
				throw new NotFoundException(nameof(CraftsMan), id);

			craftsManRepo.Save();
			return true;
		}
		public async void CreateCraftService(CreateCraftsManServiceDto serviceDto)
		{
			if (serviceDto == null)
				throw new ValidationException("Service data cannot be null.");

			await generic.AddAsync(mapper.Map<CraftsManService>(serviceDto));
			generic.Save();
		}
		public void DeleteCraftsService(int id)
		{
			var service = generic.GetAsync(id).Result;
			if (service == null)
				throw new NotFoundException(nameof(CraftsManService), id);

			generic.Delete(id);
			generic.Save();
		}
	}
}
