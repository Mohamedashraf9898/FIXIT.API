using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ReviewDTOs;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Helper.UploadHandler;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Repositories.Repo;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.DAL.Models;
using FIXIT.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyReview = FIXIT.DAL.Models.Review;

namespace FIXIT.BLL.Services.Service
{
	public class CraftsManService : ICraftsManService
	{
		private readonly ICraftsManRepo craftsManRepo;
		private readonly IGenericRepository<CraftsManService> generic;
        private readonly IGenericRepository<Wallet> wallet;
        private readonly IMapper mapper;
        private readonly UploadHandler uploadHandler;
		public readonly IReviewRepository reviewRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CraftsManService(ICraftsManRepo craftsManRepo,IGenericRepository<CraftsManService> generic,
			IGenericRepository<Wallet> wallet, IMapper mapper, UploadHandler uploadHandler,IReviewRepository reviewRepository , UserManager<ApplicationUser> userManager) 
		{
			this.craftsManRepo = craftsManRepo;
			this.generic = generic;
            this.wallet = wallet;
            this.mapper = mapper;
            this.uploadHandler = uploadHandler;
			this.reviewRepository = reviewRepository;
            _userManager = userManager;
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
        public async Task CreateCraftsManAsync(CreateCraftsManDto craftsManDto)
        {
        
			if (craftsManDto == null)
                throw new ValidationException("CraftsMan data cannot be null.");
            var craftsMan = mapper.Map<CraftsMan>(craftsManDto);
            await craftsManRepo.AddAsync(craftsMan);
            craftsManRepo.Save();
			var craftsmanwallet = new Wallet() { CraftsManId = craftsMan.Id, Balance = 0 };
			await wallet.AddAsync(craftsmanwallet);
			wallet.Save();
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
		public async Task<CraftsManDetailsDto> GetCraftsManByEmailAsync(string Email)
		{
            var normalizedEmail = Email.ToUpper();
            var craftsMan = await craftsManRepo.GetCraftsManByEmailAsync(normalizedEmail);

            if (craftsMan == null)
                throw new NotFoundException(nameof(CraftsMan), "No CraftsMan was Found");

            var reviews = await reviewRepository.GetReviewsForCraftsmanAsync(craftsMan.Id);
            if (reviews == null)
                // This might be acceptable (empty list) rather than exception, but sticking to existing pattern if preferred.
                // However, user just wants to fix the NRE.
                throw new NotFoundException(nameof(reviews), "No reviews was Found");

            return new CraftsManDetailsDto
            {
                CraftsMan = mapper.Map<CraftsManDto>(craftsMan),
                Reviews = mapper.Map<IEnumerable<GetAllReviewsDTO>>(reviews)
            };
            return new CraftsManDetailsDto
            {
                CraftsMan = mapper.Map<CraftsManDto>(craftsMan),
                Reviews = mapper.Map<IEnumerable<GetAllReviewsDTO>>(reviews)
            };
		}
		//public async Task CreateCraftsManAsync(CreateCraftsManDto craftsManDto)
		//{

		//	if (craftsManDto == null)
		//		throw new ValidationException("Craftsman data cannot be null.");

		//	await craftsManRepo.AddAsync(mapper.Map<CraftsMan>(craftsManDto));
		//	craftsManRepo.Save();
		//}


        public void DeleteCraftsMan(int id)
		{
			var craftsMan = craftsManRepo.GetAsync(id).Result;
			if (craftsMan == null)
				throw new NotFoundException(nameof(CraftsMan), id);

			craftsManRepo.Delete(id);
			craftsManRepo.Save();
		}

        public async Task<bool> UpdateCraftsmanVerfication(VerficationOfCraftsmanDto dto,int Id)
        {

            var existingCraftsMan = await craftsManRepo.GetAsync(Id);
            if (existingCraftsMan == null)
                return false;
            existingCraftsMan.IsVerified = dto.IsVerified;
           
            var updated = craftsManRepo.Update(existingCraftsMan, Id);
            if (updated)
            {
                craftsManRepo.Save();
                return true;
            }

            return false;
        }
        //public async Task<bool> UpdateCraftsManAsync(int id, UpdateCraftsManDto craftsManDto)
        //{
        //    // 1️⃣ Get the existing craftsman from DB
        //    var existingCraftsMan = await craftsManRepo.GetAsync(id);
        //    if (existingCraftsMan == null)
        //        return false;

        //    // 2️⃣ Map updated data from DTO → existing entity
        //    mapper.Map(craftsManDto, existingCraftsMan);

        //    // 3️⃣ Handle picture upload
        //    if (craftsManDto.ProfileImage != null)
        //    {
        //        // (Optional) Delete old picture file if it exists
        //        if (!string.IsNullOrEmpty(existingCraftsMan.ProfileImage))
        //        {
        //            var oldPath = Path.Combine("wwwroot", existingCraftsMan.ProfileImage);
        //            if (System.IO.File.Exists(oldPath))
        //                System.IO.File.Delete(oldPath);
        //        }

        //        // Upload new image using your file service
        //        existingCraftsMan.ProfileImage = uploadHandler.Upload(craftsManDto.ProfileImage);
        //    }
        //    // Handle NationalIdPic upload
        //    if (craftsManDto.NationalIdPic != null)
        //    {
        //        if (!string.IsNullOrEmpty(existingCraftsMan.NationalIdPic))
        //        {
        //            var oldPath = Path.Combine("wwwroot", existingCraftsMan.NationalIdPic);
        //            if (System.IO.File.Exists(oldPath))
        //                System.IO.File.Delete(oldPath);
        //        }
        //        existingCraftsMan.NationalIdPic = uploadHandler.Upload(craftsManDto.NationalIdPic, "NationalIdPics");
        //    }

        //    // 4️⃣ Update in repository
        //    var updated = craftsManRepo.Update(existingCraftsMan, id);
        //    if (updated)
        //    {
        //        craftsManRepo.Save();
        //        return true;
        //    }

        //    return false;
        //}
        public async Task<bool> UpdateCraftsManAsync(int id, UpdateCraftsManDto craftsManDto)
        {
            // 1️⃣ Get CraftsMan
            var existingCraftsMan = await craftsManRepo.GetAsync(id);
            if (existingCraftsMan == null)
                return false;

            // 2️⃣ Update CraftsMan table
            mapper.Map(craftsManDto, existingCraftsMan);

            // 3️⃣ Profile image
            if (craftsManDto.ProfileImage != null)
            {
                if (!string.IsNullOrEmpty(existingCraftsMan.ProfileImage))
                {
                    var oldPath = Path.Combine("wwwroot", existingCraftsMan.ProfileImage);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                existingCraftsMan.ProfileImage = uploadHandler.Upload(craftsManDto.ProfileImage);
            }

            // 4️⃣ National ID pic
            if (craftsManDto.NationalIdPic != null)
            {
                if (!string.IsNullOrEmpty(existingCraftsMan.NationalIdPic))
                {
                    var oldPath = Path.Combine("wwwroot", existingCraftsMan.NationalIdPic);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                existingCraftsMan.NationalIdPic =
                    uploadHandler.Upload(craftsManDto.NationalIdPic, "NationalIdPics");
            }

            craftsManRepo.Update(existingCraftsMan, id);
            craftsManRepo.Save();

            // 5️⃣ Update AspNetUsers (Identity)
            var user = await _userManager.FindByEmailAsync(existingCraftsMan.NormalizedEmail);
            if (user != null)
            {
                user.FName = craftsManDto.FName;
                user.LName = craftsManDto.LName;
                user.PhoneNumber = craftsManDto.PhoneNumber;


                await _userManager.UpdateAsync(user);
            }

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

        public async Task<CraftsManDetailsDto> GetCraftsManDetailsAsync(int id)
        {
            var craftsman = await craftsManRepo.GetAsync(id)?? throw new NotFoundException(nameof(CraftsMan), id);

            var reviews = await reviewRepository.GetReviewsForCraftsmanAsync(id);

            return new CraftsManDetailsDto
            {
                CraftsMan = mapper.Map<CraftsManDto>(craftsman),
                Reviews = mapper.Map<IEnumerable<GetAllReviewsDTO>>(reviews)
            };
        }


    }
}
