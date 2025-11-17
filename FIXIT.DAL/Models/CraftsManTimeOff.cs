using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{
    public class CraftsManTimeOff
    {
        public int Id { get; set; }

        // RELATIONSHIP: Links to the craftsman
        [Required]
        public int CraftsManId { get; set; }
        public virtual CraftsMan CraftsMan { get; set; }

        // DATE RANGE: When time off starts
        // Example: 2025-12-20 00:00:00
        [Required]
        public DateTime StartDate { get; set; }

        // DATE RANGE: When time off ends
        // Example: 2025-12-27 23:59:59
        [Required]
        public DateTime EndDate { get; set; }

        // TYPE: What kind of time off
        [Required]
        public TimeOffType Type { get; set; }

        // REASON: Optional description
        // Example: "Family vacation" or "Sick - flu"
        [MaxLength(500)]
        public string? Reason { get; set; }

        // APPROVAL: For future workflow if admin approval needed
        // For now, always true (auto-approved)
        public bool IsApproved { get; set; } = true;

        // AUDIT: When this was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Types of time off
    /// </summary>
    public enum TimeOffType
    {
        Vacation,    // Planned vacation
        Sick,        // Sick leave
        Personal,    // Personal day
        Emergency,   // Emergency leave
        Holiday,     // Public holiday
        Other        // Other reasons
    }

}
