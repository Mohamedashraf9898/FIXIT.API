using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FIXIT.BLL.DTOs.CraftsmanDTOs
{
    public class IdVerificationRequestDto
    {
        public IFormFile FrontImage { get; set; }
        public IFormFile BackImage { get; set; }
        public string Email { get; set; } // Craftsman email
    }

    public class IdVerificationResponseDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public ExtractedDataDto ExtractedData { get; set; } = new();
    }

    public class ExtractedDataDto
    {
        public string? FullName { get; set; }
        public string? NationalIdNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public bool IsExpired { get; set; }
    }
}
