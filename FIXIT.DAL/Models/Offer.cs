using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public enum OfferStatus
    {
        Pending,              
        AcceptedByClient,     
        RejectedByClient,     
        AcceptedByCraftsman,  
        RejectedByCraftsman   
    }

    public class Offer
    {
        public int Id { get; set; }

        public int ServiceRequestId { get; set; }
        public ServicesRequest ServiceRequest { get; set; }

        public int CraftsmanId { get; set; }
        public CraftsMan Craftsman { get; set; }

        public decimal Amount { get; set; }

        public OfferStatus Status { get; set; } = OfferStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
