using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIXIT.DAL.Models
{
    public enum ServiceRequestStatus
    {   Pending,
        InProgress,
        Completed,
        Cancelled
    }
    public class ServicesRequest
    {
        public int ServicesRequestId { get; set; }
        public string Description { get; set; }
        public string ServiceRequestImage { get; set; }
        public int CraftsManId { get; set; }
        public int ClientId { get; set; }
        public int ServiceId { get; set; }

        public virtual CraftsMan CraftsMan { get; set; }
        public virtual Client Client { get; set; }
        public virtual Service Service { get; set; }

        public DateTime RequestAt { get; set; }
        public DateTime ServiceAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public string Location { get; set; }

        [DefaultValue(ServiceRequestStatus.Pending)]
        public ServiceRequestStatus Status { get; set; }
        public virtual Review Review { get; set; }
      
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        public virtual WalletTransaction WalletTransaction { get; set; }
    }
}
