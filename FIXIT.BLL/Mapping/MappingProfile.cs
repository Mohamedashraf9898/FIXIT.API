using AutoMapper;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ReviewDTOs;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.DTOs.ServicsDTOs;
using FIXIT.BLL.DTOs.OfferDto;
using FIXIT.BLL.DTOs.ReviewDTOs;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.DTOs.WalletDTos;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile() 
		{
            // ============================
            //CraftsMan mapping
            // ============================
            CreateMap<CraftsMan,CraftsManDto>().ReverseMap();
			CreateMap<CraftsMan,CreateCraftsManDto>().ReverseMap();
			CreateMap<CraftsMan, UpdateCraftsManDto>().ReverseMap();
			CreateMap<CraftsManService,CreateCraftsManServiceDto>().ReverseMap();
            // ============================
            //CLient Maping 
            // ============================
            CreateMap<Client,GetAllClientsDTO>().ReverseMap();
			CreateMap<Client, CreateClientDTO>().ReverseMap();
			CreateMap<Client, UpdateClientDTO>().ReverseMap();

            //Service Mapping 
            CreateMap<Service, GetAllServicesDTO>().ReverseMap();
            CreateMap<Service, CreateServiceDto>().ReverseMap();
            CreateMap<Service, UpdateServiceDto>().ReverseMap();
            CreateMap<Service, ServiceDto>().ReverseMap();
            //Review mapping
            CreateMap<Review, CreateReviewDTO>().ReverseMap();
            CreateMap<Review, GetAllReviewsDTO>().ReverseMap();
            CreateMap<Review, UpdateReviewDTO>().ReverseMap();
            CreateMap<CreateReviewDTO, Review>()


                .ForMember(
                    dest => dest.ReviewDate,
                    opt => opt.MapFrom(src => DateTime.UtcNow)
                )


                .AfterMap((src, dest, context) => {

                    var serviceRequest = (ServicesRequest)context.Items["ServiceRequest"];


                    dest.ClientId = serviceRequest.ClientId;
                    dest.CraftsManId = serviceRequest.CraftsManId;
                });

            
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
                    dest.ServiceRequestId = src.ServicesRequestId;
                    dest.ClientName = src.Client.FName + " " + src.Client.LName;
                    dest.ServiceName = src.Service.ServiceName;
                    dest.CraftsManName = src.CraftsMan.FName + " " + src.CraftsMan.LName;
                    dest.ReviewRatingValue = src.Review != null ? src.Review.RatingValue : 0;
					dest.ReviewComment = src.Review != null ? src.Review.Comment : string.Empty;
                    dest.Status = src.Status.ToString();
                })
				.ReverseMap();
			CreateMap<ServicesRequest, CreateServiceRequestDto>()
				.AfterMap((src , dest) =>
				{
                    dest.ClientName = src.Client.FName + " " + src.Client.LName;
                    dest.ServiceName = src.Service.ServiceName;
                    dest.CraftsManName = src.CraftsMan.FName + " " + src.CraftsMan.LName;
                    dest.ReviewRatingValue = src.Review != null ? src.Review.RatingValue : 0;
					dest.ReviewComment = src.Review != null ? src.Review.Comment : string.Empty;
                    dest.Status = src.Status.ToString();
                })
				.ReverseMap();
            // ============================
            // Review Mapping
            // ============================
            CreateMap<Review, CreateReviewDTO>().ReverseMap();
			CreateMap<Review, UpdateReviewDTO>().ReverseMap();
			CreateMap<Review, GetAllReviewsDTO>().ReverseMap();


            //========================
            //ServiceRequestMapping
            //==========================
            // CreateServiceRequestDto -> ServicesRequest
            CreateMap<CreateServiceRequestDto, ServicesRequest>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ServiceRequestStatus.Pending))
                .ForMember(dest => dest.RequestAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CraftsManId, opt => opt.Ignore()) // CraftsMan is assigned later
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.WalletTransaction, opt => opt.Ignore())
                .ForMember(dest => dest.Offers, opt => opt.Ignore())
                .ForMember(dest => dest.Review, opt => opt.Ignore())
                .ForMember(dest => dest.CraftsManId, opt => opt.Ignore());

            // ServicesRequest -> ReadServiceRequestDto
            CreateMap<ServicesRequest, ReadServiceRequestDto>()
                .ForMember(dest => dest.CraftsManName, opt => opt.MapFrom(src =>src.CraftsMan != null
                        ? $"{src.CraftsMan.FName} {src.CraftsMan.LName}": null))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src =>src.Client != null
                        ? $"{src.Client.FName} {src.Client.LName}": null))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src =>src.Service != null ? src.Service.ServiceName : null))
                .ForMember(dest => dest.ReviewRatingValue, opt => opt.MapFrom(src =>src.Review != null ? src.Review.RatingValue : (int?)null))
                .ForMember(dest => dest.ReviewComment, opt => opt.MapFrom(src =>src.Review != null ? src.Review.Comment : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>src.Status.ToString()))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                           src.Location != null && src.Location != ""
                               ? src.Location
                               : src.Client != null
                                   ? src.Client.Location
                                   : null ));

            // UpdateServiceRequestDto -> ServicesRequest
            CreateMap<UpdateServiceRequestDto, ServicesRequest>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // update only non-null fields

            // ============================
            // Offer Mappings
            // ============================

            // CraftsManNewOfferDto -> Offer
            CreateMap<CraftsManNewOfferDto, Offer>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OfferStatus.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CraftsmanId, opt => opt.Ignore()) // assign manually
                .ForMember(dest => dest.ServiceRequestId, opt => opt.MapFrom(src => src.ServiceRequestId));

            // ClientRespondDto -> Offer (partial mapping, handled in service)
            CreateMap<ClientRespondDto, Offer>();

            // CraftsmanAcceptDto & CraftsmanRejectDto -> Offer
            CreateMap<CraftsmanAcceptDto, Offer>();
            CreateMap<CraftsmanRejectDto, Offer>();

            // ClientSelectCraftsmanDto -> ServicesRequest
            //CreateMap<ClientSelectCraftsmanDto, ServicesRequest>()
            //    .ForMember(dest => dest.CraftsManId, opt => opt.MapFrom(src => src.CraftsmanId))
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ServiceRequestStatus.InProgress));
            CreateMap<ClientSelectCraftsmanDto, Offer>()
    .ForMember(dest => dest.CraftsmanId, opt => opt.MapFrom(src => src.CraftsmanId))
    .ForMember(dest => dest.ServiceRequestId, opt => opt.MapFrom(src => src.ServiceRequestId))
    .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => 0)) // الحرفي بعد كده يحدد السعر
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OfferStatus.Pending))
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
    .ForMember(dest => dest.SuggestedPrice, opt => opt.Ignore());
            // CraftsManNewOfferDto ↔ Offer
            CreateMap<CraftsManNewOfferDto, Offer>()
     .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.NewAmount))
     .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OfferStatus.Pending))
     .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
     .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
     .ForMember(dest => dest.SuggestedPrice, opt => opt.MapFrom(src => src.SuggestedPrice));
            // ============================
            //wallet
            // ============================
            CreateMap<Wallet, WalletDto>().ReverseMap();
            CreateMap<CreateWalletDto, Wallet>().ReverseMap();

            CreateMap<WalletTransaction, WalletTransactionDto>().ReverseMap();
            CreateMap<CreateWalletTransactionDto, WalletTransaction>().ReverseMap();

        }
	}
}
