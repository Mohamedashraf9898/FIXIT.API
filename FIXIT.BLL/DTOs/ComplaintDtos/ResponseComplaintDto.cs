using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ComplaintDtos
{
    public class ResponseComplaintDto
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public int ClientId { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
