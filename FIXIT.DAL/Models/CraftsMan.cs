using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public class CraftsMan
    {
        public string Bio { get; set; }
        public string ProfileImage { get; set; }
        public int ExperienceOfYears { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsVerified { get; set; }
        
    }
}
