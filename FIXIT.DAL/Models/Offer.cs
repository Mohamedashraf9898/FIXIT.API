using FIXIT.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;

public enum OfferStatus
{
    Pending,
    NewOfferFromCraftsman,
    AcceptedByClient,
    RejectedByClient,
    AcceptedByCraftsman,
    RejectedByCraftsman,
    Cancelled
}

public class Offer
{
    public int Id { get; set; }

    public int ServiceRequestId { get; set; }
    public virtual ServicesRequest ServiceRequest { get; set; }

    public int CraftsmanId { get; set; }
    public virtual CraftsMan Craftsman { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal? SuggestedPrice { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    public string Description { get; set; }

    public OfferStatus Status { get; set; } = OfferStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
