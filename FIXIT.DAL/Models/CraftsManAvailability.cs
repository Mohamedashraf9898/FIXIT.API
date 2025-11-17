using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public class CraftsManAvailability
    {
        public int Id { get; set; }

        // RELATIONSHIP: Links to the craftsman
        [Required]
        public int CraftsManId { get; set; }
        public virtual CraftsMan CraftsMan { get; set; }

        // WHICH DAY: 0=Sunday, 1=Monday, 2=Tuesday, etc.
        // Example: DayOfWeek.Monday = 1
        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        // WORKING HOURS: When craftsman starts work
        // Example: 09:00:00 means 9 AM
        // Stored as TimeSpan (time duration from midnight)
        [Required]
        public TimeSpan StartTime { get; set; }

        // WORKING HOURS: When craftsman finishes work
        // Example: 17:00:00 means 5 PM
        [Required]
        public TimeSpan EndTime { get; set; }

        // IS AVAILABLE: Set to false to mark day as OFF
        // Example: Sunday might be IsAvailable = false
        public bool IsAvailable { get; set; } = true;

        // AUDIT: When this availability was created/updated
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
