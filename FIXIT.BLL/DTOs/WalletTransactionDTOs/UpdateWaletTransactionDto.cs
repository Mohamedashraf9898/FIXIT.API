using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.WalletTransactionDTOs
{
    public class UpdateWaletTransactionDto
    {
        public int Id { get; set; }
        public bool? ispayed { get; set; }
    }
}
