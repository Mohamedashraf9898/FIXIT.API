using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.DTOs.WalletTransactionDTOs
{

    public class CreateWalletTransactionDto
    {
        public int CraftsManId { get; set; }
        public int WalletId { get; set; }
        public decimal? Amount { get; set; }
        public Transactionmethod? Transactionmethod { get; set; }
        public TransactionType? Transactiontype { get; set; }
        public string? TransationInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ServiceRequestId { get; set; }
    }
}
