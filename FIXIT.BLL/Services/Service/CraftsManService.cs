
using AutoMapper;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Repositories.IRepo;
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
		private readonly IGenericRepository<CraftsMan> genericRepository;
		private readonly IMapper mapper;

		public CraftsManService(IGenericRepository<CraftsMan> genericRepository,IMapper mapper) 
		{
			this.genericRepository = genericRepository;
			this.mapper = mapper;
		}
		public async Task<IEnumerable<CraftsManDto>> GetAllCraftsMenAsync()
		{
			List<CraftsMan> craftsMen = await genericRepository.GetAllAsync();
			var result = mapper.Map<List<CraftsManDto>>(craftsMen);
			return result;
		}
		public async Task<CraftsManDto> GetCraftsManByIdAsync(int id)
		{
			CraftsMan craftsMan = await genericRepository.GetAsync(id);
			if (craftsMan is null)
				return null;	
			return mapper.Map<CraftsManDto>(craftsMan);
		}
		public async void CreateCraftsManAsync(CreateCraftsManDto craftsManDto)
		{
			
			await genericRepository.AddAsync(mapper.Map<CraftsMan>(craftsManDto));
			genericRepository.Save();
		}

		public void DeleteCraftsMan(int id)
		{
			genericRepository.Delete(id);
			genericRepository.Save();
		}

		
		public bool UpdateCraftsMan(int id,UpdateCraftsManDto craftsManDto)
		{
			if(	genericRepository.Update(mapper.Map<CraftsMan>(craftsManDto),id))
			{
                genericRepository.Save();
				return true;
            }
			return false;
		}

		
	}
}
