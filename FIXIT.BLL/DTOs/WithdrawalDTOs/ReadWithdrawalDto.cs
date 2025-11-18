using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.WithdrawalDTOs
{
    public class ReadWithdrawalDto
    {
        public int Id { get; set; }
        public int CraftsManId { get; set; }
        public decimal Amount { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
