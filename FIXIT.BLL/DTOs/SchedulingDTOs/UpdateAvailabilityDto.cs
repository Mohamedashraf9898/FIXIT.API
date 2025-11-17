using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.SchedulingDTOs
{
    public class UpdateAvailabilityDto
    {
        [Required]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")]
        public string StartTime { get; set; }

        [Required]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")]
        public string EndTime { get; set; }

        public bool IsAvailable { get; set; }
    }
}
