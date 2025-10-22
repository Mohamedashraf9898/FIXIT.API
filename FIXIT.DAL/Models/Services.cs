using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal InitialPrice { get; set; }
        public ICollection<CraftsManService> CraftsManServices { get; set; } = new HashSet<CraftsManService>();
        public ICollection<ServicesRequest> ServicesRequests { get; set; } = new HashSet<ServicesRequest>();
    }
}
