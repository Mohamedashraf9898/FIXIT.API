using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

public enum ServiceRequestStatus
{
    Pending,
    WaitingForCraftsmanResponse,
    WaitingForClientDecision,
    Approved,
    RejectedByCraftsman,
    RejectedByClient,
    InProgress,
    Completed,
    Cancelled
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
    public DateTime ServiceAt { get; set; } 
    public DateTime? CompletedAt { get; set; }

    public string Location { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? SuggestedPrice { get; set; }

    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;

    public decimal? TotalAmount { get; set; }

    public virtual ICollection<Offer> Offers { get; set; } = new HashSet<Offer>();

    public virtual Review? Review { get; set; }
    public virtual WalletTransaction? WalletTransaction { get; set; }
}
