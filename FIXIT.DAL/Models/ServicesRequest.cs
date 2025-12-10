using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIXIT.DAL.Models
{
    public enum ServiceRequestStatus
    {
        Pending,
        WaitingForCraftsmanResponse,
        WaitingForClientDecision,
        WaitingForClientPayment,
        RejectedByCraftsman,
        RejectedByClient,
        InProgress,
        Completed,
        Approved,
        Cancelled,
        CancelledDueToNonPayment,
        CancelledByCraftsman
    }
   
public class ServicesRequest
{
    
    public int ServicesRequestId { get; set; }

    public string Description { get; set; }
    
    public string ServiceRequestImage { get; set; }

    public int? CraftsManId { get; set; }
    public virtual CraftsMan? CraftsMan { get; set; }

    public int ClientId { get; set; }
    public virtual Client Client { get; set; }

    public int ServiceId { get; set; }
    public virtual Service Service { get; set; }

    public DateTime RequestAt { get; set; }
    public DateTime? CompletedAt { get; set; }

        //Abdallah
        public DateTime ServiceStartTime { get; set; } 

        [Range(15, 480, ErrorMessage = "Duration must be between 15 minutes and 8 hours")]
        public DateTime? ServiceEndTime { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
        public string Location { get; set; }
    
     [Column(TypeName = "decimal(10,2)")]
     public decimal? TotalAmount { get; set; }
        public DateTime? WaitingForClientPaymentAt { get; set; }
        public bool? IsCancelled { get; set; }


        public string? PaymentIntentId { get; set; }
    public string? ClientSecret { get; set; }
        

    [Column(TypeName = "decimal(10,2)")]
    public decimal? SuggestedPrice { get; set; }

    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;


    public virtual ICollection<Offer> Offers { get; set; } = new HashSet<Offer>();

    public virtual Review? Review { get; set; }
    public virtual WalletTransaction? WalletTransaction { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}
