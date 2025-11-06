using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Helper.PictureUrlResolver
{
    public class PictureUrlResolver<TSource, TDestination>
     : IValueResolver<TSource, TDestination, string>
    {
        private readonly IConfiguration _config;

        public PictureUrlResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
        {
            
            var prop = typeof(TSource).GetProperty("PictureUrl");
            if (prop != null)
            {
                var value = prop.GetValue(source)?.ToString();
                if (!string.IsNullOrEmpty(value))
                    return $"{_config["ApiUrl"]}{value}";
            }

            return string.Empty;
        }
    }

}
