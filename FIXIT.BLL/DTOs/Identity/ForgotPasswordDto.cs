using System.ComponentModel.DataAnnotations;

namespace FIXIT.BLL.DTOs.Identity
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
