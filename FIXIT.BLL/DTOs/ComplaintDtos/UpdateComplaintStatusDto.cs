namespace FIXIT.BLL.DTOs.ComplaintDtos
{
    public class UpdateComplaintStatusDto
    {
        public int ComplaintId { get; set; }
        public string Status { get; set; } // e.g. "Resolved"
    }
}
