using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.Identity;
using FIXIT.BLL.DTOs.ReviewDTOs;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.DTOs.WalletDTos;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.DAL.Models;
using FIXIT.DAL.Models.Identity;

namespace FIXIT.BLL.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile() 
		{
			//CraftsMan mapping
			CreateMap<CraftsMan,CraftsManDto>().ReverseMap();
			CreateMap<CraftsMan,CreateCraftsManDto>().ReverseMap();
			CreateMap<CraftsMan, UpdateCraftsManDto>().ReverseMap();
			CreateMap<CraftsManService,CreateCraftsManServiceDto>().ReverseMap();
			//CLient Maping 
			CreateMap<Client,GetAllClientsDTO>().ReverseMap();
			CreateMap<Client, CreateClientDTO>().ReverseMap();
			CreateMap<Client, UpdateClientDTO>().ReverseMap();
            // Review Mapping
			CreateMap<Review, CreateReviewDTO>().ReverseMap();
			CreateMap<Review, UpdateReviewDTO>().ReverseMap();
			CreateMap<Review, GetAllReviewsDTO>().ReverseMap();


            //ServiceRequestMapping
            CreateMap<ServicesRequest, ReadServiceRequestDto>()
				.AfterMap((src , dest) =>
				{
					dest.ServiceRequestId = src.ServicesRequestId;
                    dest.CraftsManName = src.CraftsMan.FName + " " + src.CraftsMan.LName;
					dest.ClientName = src.Client.FName + " " + src.Client.LName;
					dest.ServiceName = src.Service.ServiceName;
					dest.ReviewRatingValue = src.Review != null ? src.Review.RatingValue : 0;
					dest.ReviewComment = src.Review != null ? src.Review.Comment : string.Empty;
					dest.Status = src.Status.ToString();
                })
				.ReverseMap();
			CreateMap<ServicesRequest, UpdateServiceRequestDto>()
				.AfterMap((src , dest) =>
				{
                    dest.Status = src.Status.ToString();
                })
				.ReverseMap();
			CreateMap<ServicesRequest, CreateServiceRequestDto>()
				.AfterMap((src , dest) =>
				{
                    dest.Status = src.Status.ToString();
                })
				.ReverseMap();
            //wallet
            CreateMap<Wallet, WalletDto>().ReverseMap();
            CreateMap<CreateWalletDto, Wallet>().ReverseMap();

            CreateMap<WalletTransaction, WalletTransactionDto>().ReverseMap();
            CreateMap<CreateWalletTransactionDto, WalletTransaction>().ReverseMap();

			//identity 

			//CreateMap<ApplicationUser, UserDto>().ReverseMap();
			//CreateMap<ClientRegisterDto, ApplicationUser>().ReverseMap();
			//CreateMap<CraftsManRegisterDto, ApplicationUser>().ReverseMap();
			//CreateMap<LoginDto, ApplicationUser>().ReverseMap();

        }
	}
}
