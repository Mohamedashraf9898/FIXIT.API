using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.SchedulingDTOs
{
    public class TimeSlotDto
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime StartTime { get; set; }
    }
}
