using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public enum WithdrawalStatus
    {
        Pending,
        Success,
        Failed
    }
    public class WithdrawalRequest
    {
        public int Id { get; set; }
        public int CraftsManId { get; set; }
        public decimal Amount { get; set; }
        public string PhoneNumber { get; set; }
        public WithdrawalStatus Status { get; set; } = WithdrawalStatus.Pending;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }

        //// link to WalletTransaction
        //public int? WalletTransactionId { get; set; }
        //public virtual WalletTransaction? WalletTransaction { get; set; }
    }

   
}
