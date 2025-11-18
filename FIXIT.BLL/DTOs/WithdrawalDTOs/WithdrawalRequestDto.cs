using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.WithdrawalDTOs
{
    public class WithdrawalRequestDto
    {
        [Required]
        public decimal Amount { get; set; }
       
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}


