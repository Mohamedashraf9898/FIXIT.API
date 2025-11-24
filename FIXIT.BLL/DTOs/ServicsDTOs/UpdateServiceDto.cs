using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ServicsDTOs
{
    public class UpdateServiceDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal InitialPrice { get; set; }
        public int DisplayDurationMinutes { get; set; }
    }
}
