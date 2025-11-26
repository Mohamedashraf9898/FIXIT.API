using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.DTOs.WalletTransactionDTOs
{
    public class WalletTransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public Transactionmethod? Transactionmethod { get; set; }
        public TransactionType? Transactiontype { get; set; }
        public string? TransationInfo { get; set; }
        public DateTime CreatedAt { get; set; }

    }

}
