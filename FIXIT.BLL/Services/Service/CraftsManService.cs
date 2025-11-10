
using AutoMapper;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Helper.UploadHandler;
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
        private readonly UploadHandler uploadHandler;

        public CraftsManService(ICraftsManRepo craftsManRepo,IGenericRepository<CraftsManService> generic,IMapper mapper, UploadHandler uploadHandler) 
		{
			this.craftsManRepo = craftsManRepo;
			this.generic = generic;
			this.mapper = mapper;
            this.uploadHandler = uploadHandler;
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
            string? imagePath = null;

            if (craftsManDto.ProfileImage != null)
            {
                imagePath = uploadHandler.Upload(craftsManDto.ProfileImage, "CraftsMen");
            }

            var craftsMan = mapper.Map<CraftsMan>(craftsManDto);
            craftsMan.ProfileImage = imagePath;

            await craftsManRepo.AddAsync(craftsMan);
            craftsManRepo.Save();
        }


        public void DeleteCraftsMan(int id)
		{
			craftsManRepo.Delete(id);
			craftsManRepo.Save();
		}


        public async Task<bool>  UpdateCraftsManAsync(int id, UpdateCraftsManDto craftsManDto)
        {
            // 1️⃣ Get the existing craftsman from DB
            var existingCraftsMan = await craftsManRepo.GetAsync(id);
            if (existingCraftsMan == null)
                return false;

            // 2️⃣ Map updated data from DTO → existing entity
            mapper.Map(craftsManDto, existingCraftsMan);

            // 3️⃣ Handle picture upload
            if (craftsManDto.ProfileImage != null)
            {
                // (Optional) Delete old picture file if it exists
                if (!string.IsNullOrEmpty(existingCraftsMan.ProfileImage))
                {
                    var oldPath = Path.Combine("wwwroot", existingCraftsMan.ProfileImage);
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                // Upload new image using your file service
                existingCraftsMan.ProfileImage = uploadHandler.Upload(craftsManDto.ProfileImage);
            }

            // 4️⃣ Update in repository
            var updated = craftsManRepo.Update(existingCraftsMan, id);
            if (updated)
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
