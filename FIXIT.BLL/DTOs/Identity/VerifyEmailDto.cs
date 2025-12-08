using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.Identity
{
    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
