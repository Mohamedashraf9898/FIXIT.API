using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using Microsoft.AspNetCore.Http;

namespace FIXIT.BLL.Services.IService
{
    public interface IOpenAIService
    {
        Task<IdVerificationResponseDto> VerifyEgyptianNationalIdAsync(
            IFormFile frontImage,
            IFormFile backImage);
    }
}
