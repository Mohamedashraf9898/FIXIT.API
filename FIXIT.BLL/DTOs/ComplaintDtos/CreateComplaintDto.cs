using System;

namespace FIXIT.BLL.DTOs.ComplaintDtos
{
    public class CreateComplaintDto
    {
        public int ServiceRequestId { get; set; }
        public int? ClientId { get; set; }
        public int? CraftsManId { get; set; }
        public string Content { get; set; }
    }

   
}
