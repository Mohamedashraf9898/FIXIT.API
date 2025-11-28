using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Helper.PictureUrlResolver
{
    public class ServiceRequestImageResolver<TSource, TDestination>
     : IValueResolver<TSource, TDestination, string?>
    {
        private readonly IConfiguration _config;

        public ServiceRequestImageResolver(IConfiguration config)
        {
            _config = config;
        }

        public string? Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
        {
            var prop = typeof(TSource).GetProperty("ServiceRequestImage");

            if (prop == null)
                return string.Empty;

            var value = prop.GetValue(source)?.ToString();

            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return $"{_config["ApiUrl"]}{value}";
        }
    
}
}
