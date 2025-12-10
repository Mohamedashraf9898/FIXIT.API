using System.ComponentModel.DataAnnotations;

namespace FIXIT.BLL.DTOs.ServiceRequestDTOs
{
    public class CancelServiceRequestDto
    {
        [Required]
        public string Reason { get; set; }

        [Required]
        public string ReasonType { get; set; } 

        [Required]
        public string ClientName { get; set; }

        [Required, EmailAddress]
        public string ClientEmail { get; set; }

        public string? ClientPhone { get; set; }
    }
}