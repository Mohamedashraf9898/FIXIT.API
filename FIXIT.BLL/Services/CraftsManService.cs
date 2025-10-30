

using AutoMapper;
using FIXIT.BLL.DTOs;
using FIXIT.BLL.Interfaces;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services
{
	public class CraftsManService : ICraftsManService
	{
		private readonly ICraftsManRepo craftsManRepo;
		private readonly IMapper mapper;

		public CraftsManService(ICraftsManRepo craftsManRepo,IMapper mapper) 
		{
			this.craftsManRepo = craftsManRepo;
			this.mapper = mapper;
		}
		public async Task<IEnumerable<CraftsManDto>> GetAllCraftsMenAsync()
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
		public async Task<CraftsManDto> GetCraftsByNameAsync(string fname , string lname)
		{
			CraftsMan craftsMan = await craftsManRepo.GetCraftsManByName(fname, lname);
			if (craftsMan is null)
				return null;
			return mapper.Map<CraftsManDto>(craftsMan);
		}
		public async void CreateCraftsManAsync(CreateCraftsManDto craftsManDto)
		{
			await craftsManRepo.AddAsync(mapper.Map<CraftsMan>(craftsManDto));
			craftsManRepo.Save();
		}

		public void DeleteCraftsMan(int id)
		{
			craftsManRepo.Delete(id);
			craftsManRepo.Save();
		}


		public void UpdateCraftsMan(int id,UpdateCraftsManDto craftsManDto)
		{
			craftsManRepo.Update(mapper.Map<CraftsMan>(craftsManDto));
			craftsManRepo.Save();
		}

		
	}
}
