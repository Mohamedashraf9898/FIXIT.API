using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public class CraftsMan : BaseEntity
    {
        public string Describtion { get; set; }
        public string ProfileImage { get; set; }
        public int ExperienceOfYears { get; set; }
        public decimal HourlyRate { get; set; }
        public double Rating { get; set; }
        public bool IsVerified { get; set; }
        public Service Service { get; set; }
        public int ServicesRequestId { get; set; }
        public ICollection<ServicesRequest> ServicesRequests { get; set; } = new HashSet<ServicesRequest>();

    }
}
