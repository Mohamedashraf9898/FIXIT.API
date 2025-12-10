using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ComplaintDtos
{
    public class RespondToComplaintDto
    {
        public int ComplaintId { get; set; }
        public string AdminResponse { get; set; }
        public string Status { get; set; } // e.g. "Resolved", "Rejected"
    }
}
