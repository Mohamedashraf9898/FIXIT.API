
using AutoMapper;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
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
			List<CraftsMan> craftsMen = await craftsManRepo.GetAllAsync();
			var result = mapper.Map<List<CraftsManDto>>(craftsMen);
			return result;
		}
		public async Task<CraftsManDto> GetCraftsManByIdAsync(int id)
		{
			CraftsMan craftsMan = await craftsManRepo.GetAsync(id);
			if (craftsMan is null)
				return null;	
			return mapper.Map<CraftsManDto>(craftsMan);
		}
		public async Task<List<CraftsManDto>> GetCraftsMenByNameAsync(string? fName, string? lName)
		{
			var craftsMen = await craftsManRepo.GetCraftsManByNameAsync(fName, lName);
			return mapper.Map<List<CraftsManDto>>(craftsMen);
		}
		public async Task CreateCraftsManAsync(CreateCraftsManDto craftsManDto)
		{
			
			await craftsManRepo.AddAsync(mapper.Map<CraftsMan>(craftsManDto));
			craftsManRepo.Save();
		}

		public void DeleteCraftsMan(int id)
		{
			craftsManRepo.Delete(id);
			craftsManRepo.Save();
		}

		
		public bool UpdateCraftsMan(int id,UpdateCraftsManDto craftsManDto)
		{
			if(	craftsManRepo.Update(mapper.Map<CraftsMan>(craftsManDto),id))
			{
                craftsManRepo.Save();
				return true;
            }
			return false;
		}
		public async void CreateCraftService(CreateCraftsManServiceDto serviceDto)
		{
			await generic.AddAsync(mapper.Map<CraftsManService>(serviceDto));
			generic.Save();
		}
		public void DeleteCraftsService(int id)
		{
			generic.Delete(id);
			generic.Save();
		}
	}
}
