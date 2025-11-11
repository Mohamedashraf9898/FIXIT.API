using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FIXIT.DAL.Models.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? NationalId { get; set; }
        public string? Location { get; set; }
        public string? PhoneNumberNormalized { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
       
    }
}
