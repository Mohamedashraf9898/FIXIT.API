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
       
        public ICollection<ServicesRequest> ServicesRequest  { get; set; }
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    }
}
