using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ServiceRequestDTOs
{
    public class CreateServiceRequestDto
    {
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public string Description { get; set; }
        public string? ServiceRequestImage { get; set; }
        public string? Location { get; set; }
        public DateTime ServiceAt { get; set; }
    }
}
