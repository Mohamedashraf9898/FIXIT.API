using System.ComponentModel.DataAnnotations;

namespace FIXIT.BLL.DTOs.Contact
{
    public class ContactFormDto
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        public string Message { get; set; }
    }
}