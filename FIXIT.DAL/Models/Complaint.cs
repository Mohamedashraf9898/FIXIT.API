using System;

namespace FIXIT.DAL.Models
{
    public class Complaint
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public int? ClientId { get; set; }
        public int? CraftsManId { get; set; }
        public string Content { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? AdminResponse { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}
