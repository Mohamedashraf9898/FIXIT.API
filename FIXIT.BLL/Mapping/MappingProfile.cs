using AutoMapper;
using FIXIT.BLL.DTOs;
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
			CreateMap<CraftsMan,CraftsManDto>().ReverseMap();
			CreateMap<CraftsMan,CreateCraftsManDto>().ReverseMap();
			CreateMap<CraftsMan, UpdateCraftsManDto>().ReverseMap();
		
		}
	}
}
