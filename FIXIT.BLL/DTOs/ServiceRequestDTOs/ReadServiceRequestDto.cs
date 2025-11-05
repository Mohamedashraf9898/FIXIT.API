using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ServiceRequestDTOs
{
    public class ReadServiceRequestDto
    {
        public int ServiceRequestId { get; set; }
        public string CraftsManName { get; set; }
        public string ClientName { get; set; }
        public string ServiceName { get; set; }
        public int ReviewRatingValue { get; set; }
        public string ReviewComment { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
