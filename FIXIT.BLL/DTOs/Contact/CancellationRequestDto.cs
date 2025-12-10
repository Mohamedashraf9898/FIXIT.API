using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.Contact
{
    public class CancellationRequestDto
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        public string Message { get; set; }

        public int ServiceRequestId { get; set; }

        public int? CraftsManId { get; set; }

        public int? ClientId { get; set; }

        public string ReasonType { get; set; } // "craftsman_no_show" or "cancel_request"
    }
}
