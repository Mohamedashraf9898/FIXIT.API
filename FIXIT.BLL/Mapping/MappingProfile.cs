using AutoMapper;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
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


        }
	}
}
