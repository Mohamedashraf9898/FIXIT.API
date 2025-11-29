using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public enum SlotStatus
    {
        Available = 0, 
        Locked = 1,    
        Booked = 2,    
        Cancelled = 3, 
        Disabled = 4 
    }
    public class TimeSlot
    {
        [Key]
        public int Id { get; set; }
      
        [ForeignKey("CraftsMan")]
        public int CraftsManId { get; set; }
        public virtual CraftsMan CraftsMan { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public SlotStatus Status { get; set; } = SlotStatus.Available;

        [ForeignKey("ServiceRequest")]
        public int? ServiceRequestId { get; set; }
        public virtual ServicesRequest? ServiceRequest { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PriceMultiplier { get; set; } = 1.0m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
