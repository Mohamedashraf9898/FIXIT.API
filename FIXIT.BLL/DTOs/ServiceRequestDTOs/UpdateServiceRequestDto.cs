using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ServiceRequestDTOs
{
    public class UpdateServiceRequestDto
    {
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ServiceRequestImage { get; set; }

        public string Status { get; set; }        // Use Enum string or actual Enum
        public decimal? TotalAmount { get; set; } // Nullable, only updated if client/offer accepts

    }
}
