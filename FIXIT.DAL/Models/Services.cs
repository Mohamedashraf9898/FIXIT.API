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
        public int CraftsManId { get; set; }
        public ICollection<CraftsMan> CraftsMen { get; set; } = new HashSet<CraftsMan>();

    }
}
