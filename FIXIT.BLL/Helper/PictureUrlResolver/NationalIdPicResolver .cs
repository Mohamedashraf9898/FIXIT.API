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
    public class NationalIdPicResolver : IValueResolver<CraftsMan, CraftsManDto, string?>
    {
        private readonly IConfiguration _config;

        public NationalIdPicResolver(IConfiguration config)
        {
            _config = config;
        }

        public string? Resolve(CraftsMan source, CraftsManDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.NationalIdPic)) return string.Empty;

            return $"{_config["ApiUrl"]}{source.NationalIdPic}";
        }
    }


}
