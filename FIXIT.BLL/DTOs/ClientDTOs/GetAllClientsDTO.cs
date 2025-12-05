using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Http;

namespace FIXIT.BLL.DTOs.ClientDTOs
{
    public class GetAllClientsDTO
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfileImage { get; set; } // Should be string for URL
        public Gender Gender { get; set; }
        public int TotalRequests { get; set; }

        public string NormalizedEmail { get; set; }
    }
}
