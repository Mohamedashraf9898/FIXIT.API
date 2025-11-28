using AutoMapper;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.DAL.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Helper.PictureUrlResolver
{
    public class ProfileImageResolver : IValueResolver<CraftsMan, CraftsManDto, string?>
    {
        private readonly IConfiguration _config;

        public ProfileImageResolver(IConfiguration config)
        {
            _config = config;
        }

        public string? Resolve(CraftsMan source, CraftsManDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.ProfileImage)) return string.Empty;

            return $"{_config["ApiUrl"]}{source.ProfileImage}";
        }
    }

}
