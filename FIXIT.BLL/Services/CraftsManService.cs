

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
		private readonly IGenericRepository<CraftsMan> genericRepository;

		public CraftsManService(IGenericRepository<CraftsMan> genericRepository) 
		{
			this.genericRepository = genericRepository;
		}
		public async Task<IEnumerable<CraftsManDto>> GetAllCraftsMenAsync()
		{
			List<CraftsMan> craftsMen = await genericRepository.GetAllAsync();
			List<CraftsManDto> craftsManDtos = new List<CraftsManDto>();

			foreach (var craftsMan in craftsMen)
			{
				CraftsManDto craftsManDto = new CraftsManDto
				{
					FName = craftsMan.FName,
					LName = craftsMan.LName,
					Describtion = craftsMan.Describtion,
					ProfileImage = craftsMan.ProfileImage,
					Rating = craftsMan.Rating
				};
				craftsManDtos.Add(craftsManDto);
			}
		
			return craftsManDtos;
		}
		public async Task<CraftsManDto> GetCraftsManByIdAsync(int id)
		{
			CraftsMan craftsMan = await genericRepository.GetAsync(id);
			if (craftsMan is null)
				return null;	
			CraftsManDto craftsManDto = new CraftsManDto
			{
				FName = craftsMan.FName,
				LName = craftsMan.LName,
				Describtion = craftsMan.Describtion,
				ProfileImage = craftsMan.ProfileImage,
				Rating = craftsMan.Rating
			};
			
			return craftsManDto;
		}
		public async void CreateCraftsManAsync(CreateCraftsManDto craftsMan)
		{
			CraftsMan newCraftsMan = new CraftsMan
			{
				FName = craftsMan.FName,
				LName = craftsMan.LName,
				NationalId = craftsMan.NationalId,
				Location = craftsMan.Location,
				PhoneNumber = craftsMan.PhoneNumber,
				Gender = craftsMan.Gender,
				DateOfBirth = craftsMan.DateOfBirth,
				Describtion = craftsMan.Describtion,
				ProfileImage = craftsMan.ProfileImage,
				ExperienceOfYears = craftsMan.ExperienceOfYears,
				HourlyRate = craftsMan.HourlyRate
			};
			await genericRepository.AddAsync(newCraftsMan);
			genericRepository.Save();
		}

		public void DeleteCraftsManAsync(int id)
		{
			genericRepository.Delete(id);
			genericRepository.Save();
		}


		public void UpdateCraftsManAsync(int id, UpdateCraftsManDto craftsManDto)
		{
			CraftsMan craftsMan = new CraftsMan
			{
				Id = id,
				FName = craftsManDto.FName,
				LName = craftsManDto.LName,
				Describtion = craftsManDto.Describtion,
				ProfileImage = craftsManDto.ProfileImage,
				ExperienceOfYears = craftsManDto.ExperienceOfYears,
				HourlyRate = craftsManDto.HourlyRate
			};
			genericRepository.Update(craftsMan);
			genericRepository.Save();
		}
	}
}
