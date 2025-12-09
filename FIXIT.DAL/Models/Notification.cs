using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public enum NotificationSenderType
    {
        Client,
        Craftsman,
        Admin
    }

    public enum NotificationType
    {
        SelectCraftsman,
        CraftsmanAccepted,
        CraftsmanRejected,
        NewOfferFromCraftsman,
        ClientAcceptedOffer,
        ClientRejectedOffer,
        PaymentRequested,
        WithdrawalRequested,     
        WithdrawalApproved,
        ServiceCancelled,          
        CraftsmanNoShow
    }
    public class Notification
    {

        public int Id { get; set; }
        public int? CraftsManId { get; set; }  
        public int? ClientId { get; set; }
        public int? ServiceRequestId { get; set; }
        public virtual ServicesRequest? ServiceRequest { get; set; }
        public int? OfferId { get; set; }            
        public virtual Offer? Offer { get; set; }     
        public string Title { get; set; }
        public string Message { get; set; }
        public decimal? FinalAmount { get; set; }
        public string? Description { get; set; }
        public NotificationSenderType SenderType { get; set; }
        public NotificationType Type { get; set; }
        
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
