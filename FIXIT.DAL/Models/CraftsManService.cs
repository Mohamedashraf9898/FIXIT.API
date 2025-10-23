using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public class CraftsManService
    {
        public int CraftsManId { get; set; }
        public CraftsMan CraftsMan { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public decimal HourlyRate { get; set; }
    }
}
