using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.DTOs.Identity
{
    public class ClientRegisterDto
    {
        [Required]
        public required string FName { get; set; }
        [Required]
        public  required string LName { get; set; }
        
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
    ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public required string Password { get; set; }
        [Required]
        public required string Location { get; set; }
        [Required]
        public required string PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
        [Required]
        public required Gender Gender { get; set; }
        [Required]
        public required DateTime DateOfBirth { get; set; }

    }
}
