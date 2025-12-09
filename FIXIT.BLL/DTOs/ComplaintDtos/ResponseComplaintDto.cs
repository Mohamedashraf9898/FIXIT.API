using System;

namespace FIXIT.BLL.DTOs.ComplaintDtos
{
    public class ResponseComplaintDto
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public int? ClientId { get; set; }
        public int? CraftsManId { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AdminResponse { get; set; }
        public DateTime? RespondedAt { get; set; }
    }

    public class RespondToComplaintDto
    {
        public int ComplaintId { get; set; }
        public string AdminResponse { get; set; }
        public string Status { get; set; } // e.g. "Resolved", "Rejected"
    }
}
