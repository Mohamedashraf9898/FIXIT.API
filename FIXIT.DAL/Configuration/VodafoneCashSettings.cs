using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Configuration
{
    public class VodafoneCashSettings
    {
        public string ApiBaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string MerchantId { get; set; }
        public string PinCode { get; set; }
    }

}
