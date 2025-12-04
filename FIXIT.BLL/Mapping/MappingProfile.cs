using AutoMapper;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.NotificationDtos;
using FIXIT.BLL.DTOs.OfferDto;
using FIXIT.BLL.DTOs.ReviewDTOs;
using FIXIT.BLL.DTOs.SchedulingDTOs;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.DTOs.ServicsDTOs;
using FIXIT.BLL.DTOs.WalletDTos;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Helper.PictureUrlResolver;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile() 
		{
			//CraftsMan mapping
			CreateMap<CraftsMan, CraftsManDto>()
                .ForMember(dest => dest.ServiceName,
                    opt => opt.MapFrom(src => src.Service != null ? src.Service.ServiceName : null))
                .ForMember(dest => dest.ProfileImage,
        opt => opt.MapFrom<ProfileImageResolver>())
                .ForMember(dest => dest.NationalIdPic,
        opt => opt.MapFrom<NationalIdPicResolver>())
                .ForMember(dest => dest.AverageRating,
        opt => opt.MapFrom(src => src.Reviews != null && src.Reviews.Any() ? src.Reviews.Average(r => (double)r.RatingValue) : 0.0))
                .ReverseMap();
			CreateMap<CraftsMan,CreateCraftsManDto>().ReverseMap();
            CreateMap<CraftsMan, UpdateCraftsManDto>().ReverseMap()
    .ForMember(dest => dest.ProfileImage, opt => opt.Condition(src => src.ProfileImage != null));
     
     
            //CLient Maping 
            // CLient Maping 
            CreateMap<Client, GetAllClientsDTO>().ForMember(dest => dest.ProfileImage,
                opt => opt.MapFrom<ClientProfileImageResolver>()).ReverseMap();
            CreateMap<Client, CreateClientDTO>().ReverseMap();
    
			CreateMap<Client, UpdateClientDTO>().ReverseMap()
                .ForMember(dest => dest.ProfileImage, opt => opt.Condition(src => src.ProfileImage != null));

            //Service Mapping 
            CreateMap<Service, GetAllServicesDTO>().ReverseMap();
            CreateMap<Service, CreateServiceDto>().ReverseMap();
            CreateMap<Service, UpdateServiceDto>().ReverseMap();
            CreateMap<Service, ServiceDto>().ReverseMap();
    
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
      .ForMember(dest => dest.ServiceRequestImage,
          opt => opt.MapFrom<ServiceRequestImageResolver<CreateServiceRequestDto, ServicesRequest>>());

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


            //
            CreateMap<ServicesRequest , ReturnedServiceRequestDto>().ReverseMap();
            //
            CreateMap<Offer , ReturnedOfferDto>().ReverseMap();
            //
                CreateMap<Offer , ReadOfferId>().ReverseMap();

            // ServicesRequest -> ReadServiceRequestDto
            CreateMap<ServicesRequest, ReadServiceRequestDto>()
       .ForMember(dest => dest.ServiceRequestImage,
           opt => opt.MapFrom<ServiceRequestImageResolver<ServicesRequest, ReadServiceRequestDto>>())

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
     .ForMember(dest => dest.ServiceRequestImage,
         opt => opt.MapFrom<ServiceRequestImageResolver<UpdateServiceRequestDto, ServicesRequest>>());
            // update only non-null fields


            //

            CreateMap<ServicesRequest,ConfirmStartatTimeDto>().ReverseMap();

            //
            CreateMap<Notification, CreateNotificationDto>()
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ServiceRequest.ClientId))
                .ForMember(dest => dest.CraftsManId, opt => opt.MapFrom(src => src.ServiceRequest.CraftsManId));


            //
            CreateMap<CreateNotificationDto, Notification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceRequest, opt => opt.Ignore()) // لأنها navigation property
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Notification, ReadNotificationDto>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))  // ✅ Map Id
              .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))  // ✅ Map Type
              .ForMember(dest => dest.ServiceRequestId, opt => opt.MapFrom(src => src.ServiceRequestId))
              .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
              .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
              .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
              .ForMember(dest => dest.OfferId, opt => opt.MapFrom(src => src.OfferId))
              .ForMember(dest => dest.FinalAmount, opt => opt.MapFrom(src => src.FinalAmount))
              .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            





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
     .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.FinalAmount))
     .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OfferStatus.Pending))
     .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
     .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            // ============================
            //wallet
            // ============================
            CreateMap<Wallet, WalletDto>().ReverseMap();
            CreateMap<CreateWalletDto, Wallet>().ReverseMap();

            CreateMap<WalletTransaction, WalletTransactionDto>().ReverseMap();
           
            CreateMap<UpdateWaletTransactionDto, WalletTransaction>().ReverseMap();

            CreateMap<CreateWalletTransactionDto, WalletTransaction>()
            .ForMember(dest => dest.CraftsManId, opt => opt.MapFrom(src => src.CraftsManId)).ReverseMap();

            // ============================
            // Availability Mapping
            // ============================
            CreateMap<CraftsManAvailability, AvailabilityDto>()
                .ForMember(dest => dest.DayName, opt => opt.MapFrom(src => src.DayOfWeek.ToString()))
                .ForMember(dest => dest.StartTimeFormatted, opt => opt.MapFrom(src => src.StartTime.ToString(@"hh\:mm")))
                .ForMember(dest => dest.EndTimeFormatted, opt => opt.MapFrom(src => src.EndTime.ToString(@"hh\:mm")));
            CreateMap<CreateAvailabilityDto, CraftsManAvailability>();
            CreateMap<UpdateAvailabilityDto, CraftsManAvailability>();

            // ============================
            // Time Off Mapping
            // ============================
            CreateMap<CraftsManTimeOff, TimeOffDto>()
                .ForMember(dest => dest.TypeDescription, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => (src.EndDate - src.StartDate).Days + 1));
            CreateMap<CreateTimeOffDto, CraftsManTimeOff>();


            //////
            ///
            CreateMap<CreateNotificationDto, Notification>()
       .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.IsRead, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
       .ForMember(dest => dest.SenderType, opt => opt.Ignore());
            //identity 

            //CreateMap<ApplicationUser, UserDto>().ReverseMap();
            //CreateMap<ClientRegisterDto, ApplicationUser>().ReverseMap();
            //CreateMap<CraftsManRegisterDto, ApplicationUser>().ReverseMap();
            //CreateMap<LoginDto, ApplicationUser>().ReverseMap();

        }
	}
}
