using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;

namespace FIXIT.BLL.DTOs.WalletDTos
{
    public class WalletDto
    {
        public int Id { get; set; }
        public int CraftsManId { get; set; }
        public decimal Balance { get; set; }
        public List<WalletTransactionDto>? Transactions { get; set; }
    }
}
