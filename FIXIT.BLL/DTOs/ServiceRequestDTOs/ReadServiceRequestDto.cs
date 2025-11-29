using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Http;

namespace FIXIT.BLL.DTOs.ServiceRequestDTOs
{
    public class ReadServiceRequestDto
    {
        public int ServicesRequestId { get; set; }

        public int? CraftsManId { get; set; }
        public string? CraftsManName { get; set; } // Optional, might not be assigned yet
        public string ClientName { get; set; }
        public string ServiceName { get; set; }

        public string Description { get; set; }
        public string Location { get; set; }
        public string? ServiceRequestImage { get; set; }
        public DateTime? WaitingForClientPaymentAt { get; set; }
        public bool? IsCancelled { get; set; }

        public int? ReviewRatingValue { get; set; } // Optional, if review exists
        public string? ReviewComment { get; set; }  // Optional, if review exists

        public DateTime RequestAt { get; set; }
        public DateTime ServiceStartTime { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ServiceRequestStatus Status { get; set; }
        public decimal? SuggestedPrice { get; set; }
        public decimal? TotalAmount { get; set; }

        public string? ClientSecret { get; set; }

    }
}
