using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.Identity
{
    public class UserDto
    {
        public required int Id { get; set; }
        public required string Token { get; set; }
        public required string FName { get; set; }
        public required string LName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }

    }
}
