using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.OfferDto
{
    public enum ClientDecision
    {
        Accept,       // العميل وافق على السعر الحالي
        Reject,       // العميل رفض نهائي بدون عرض جديد
    }

    public class ClientRespondDto
    {
        public int OfferId { get; set; }
        public ClientDecision Decision { get; set; }
    }
}
