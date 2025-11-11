using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ServicsDTOs
{
    public class GetAllServicesDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string? Description { get; set; }
        public decimal InitialPrice { get; set; }
        public bool IsActive { get; set; }
    }
}
