using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ServiceRequestDTOs
{
    public class ConfirmStartatTimeDto
    {
        public int ServiceId { get; set; }
        public int ClientId { get; set; }
        public int CraftsManId { get; set; }
        public DateTime ServiceStartTime { get; set; }

    }
}
