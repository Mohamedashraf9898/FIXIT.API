using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public class Client :BaseEntity
    {
        public int TotalRequests { get; set; }
        public int ServicesRequestId { get; set; }
        public ICollection<ServicesRequest> ServicesRequest  { get; set; }
        public int CraftsManId { get; set; }
        ICollection<CraftsMan> CraftsMen { get; set; } = new HashSet<CraftsMan>();
    }
}
