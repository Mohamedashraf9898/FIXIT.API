using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.OfferDto
{
    public class CraftsmanApologizeDto
    {
        public int ServiceRequestId { get; set; }
        public string? Reason { get; set; }
    }
}
