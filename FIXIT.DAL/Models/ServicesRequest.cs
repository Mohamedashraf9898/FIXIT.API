using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public CraftsMan CraftsMan { get; set; }
        public Client Client { get; set; }
        public Service Service { get; set; }
        public DateTime RequestAt { get; set; }
        public DateTime CompletedAt { get; set; }
        [DefaultValue(ServiceRequestStatus.Pending)]
        public ServiceRequestStatus Status { get; set; }
    }
}
