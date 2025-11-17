using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.SchedulingDTOs
{
    public class CreateAvailabilityDto
    {
        [Required]
        public int CraftsManId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Start time in HH:mm format (24-hour)
        /// EXAMPLES: "09:00", "13:30", "08:00"
        /// </summary>
        [Required]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$",
            ErrorMessage = "Time must be in HH:mm format (e.g., 09:00)")]
        public string StartTime { get; set; }

        /// <summary>
        /// End time in HH:mm format (24-hour)
        /// EXAMPLES: "17:00", "18:30", "14:00"
        /// Must be after StartTime
        /// </summary>
        [Required]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$",
            ErrorMessage = "Time must be in HH:mm format (e.g., 17:00)")]
        public string EndTime { get; set; }

        /// <summary>
        /// Whether craftsman works this day
        /// Set to false for days off
        /// </summary>
        public bool IsAvailable { get; set; } = true;
    }
}
