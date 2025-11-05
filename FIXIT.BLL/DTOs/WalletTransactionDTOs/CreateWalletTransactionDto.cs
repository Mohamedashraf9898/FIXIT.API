using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.WalletTransactionDTOs
{

    public class CreateWalletTransactionDto
    {
        public int CraftsManId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        // public string TransactionType { get; set; }
        //public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ServiceRequestId { get; set; }
    }
}
