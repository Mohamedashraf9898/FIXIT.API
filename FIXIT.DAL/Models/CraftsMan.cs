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

        public ICollection<CraftsManService> CraftsManServices { get; set; } = new HashSet<CraftsManService>();
        public ICollection<ServicesRequest> ServicesRequests { get; set; } = new HashSet<ServicesRequest>();

        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    }
}
