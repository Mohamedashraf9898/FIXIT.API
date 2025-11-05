using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.WalletDTos
{
    public class CreateWalletDto
    {
        public int CraftsManId { get; set; }
        public decimal Balance { get; set; } = 0;
    }
}
