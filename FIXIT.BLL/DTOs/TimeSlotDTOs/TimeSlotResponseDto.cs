using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.TimeSlotDTOs
{
    public class TimeSlotResponseDto
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
        public decimal PriceMultiplier { get; set; }    
    }
}
