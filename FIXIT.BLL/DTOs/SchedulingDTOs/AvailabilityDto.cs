using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.SchedulingDTOs
{
    public class AvailabilityDto
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string DayName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }

        // Formatted for display
        public string StartTimeFormatted { get; set; }
        public string EndTimeFormatted { get; set; }
    }
}
