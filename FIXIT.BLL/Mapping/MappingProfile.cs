using AutoMapper;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ReviewDTOs;
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
			//CraftsMan mapping
			CreateMap<CraftsMan,CraftsManDto>().ReverseMap();
			CreateMap<CraftsMan,CreateCraftsManDto>().ReverseMap();
			CreateMap<CraftsMan, UpdateCraftsManDto>().ReverseMap();
			//Client Mapping 
			CreateMap<Client,GetAllClientsDTO>().ReverseMap();
			CreateMap<Client, CreateClientDTO>().ReverseMap();
			CreateMap<Client, UpdateClientDTO>().ReverseMap();
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

            CreateMap<Review, GetAllReviewsDTO>();

        }
	}
}
