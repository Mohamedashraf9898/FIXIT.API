using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.OfferDto
{
    public class CraftsManNewOfferDto
    {
        
        public int ServiceRequestId { get; set; }
        public int CraftsmanId { get; set; }
        public decimal FinalAmount { get; set; }
        public string Description { get; set; }
    }
}
