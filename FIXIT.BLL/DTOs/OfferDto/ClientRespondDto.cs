using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.OfferDto
{
    public enum ClientDecision
    {
        Accept,       
        Reject,          
    }

    public class ClientRespondDto
    {
        public int OfferId { get; set; }
        public ClientDecision Decision { get; set; }
    }
}
