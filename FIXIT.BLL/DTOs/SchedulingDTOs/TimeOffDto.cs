using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.SchedulingDTOs
{
    public class TimeOffDto
    {
        public int Id { get; set; }
        public int CraftsManId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeOffType Type { get; set; }
        public string TypeDescription { get; set; }
        public string? Reason { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DurationDays { get; set; }
    }
    public enum TimeOffType
    {
        Vacation = 0,
        Sick = 1,
        Personal = 2,
        Emergency = 3,
        Holiday = 4,
        Other = 5
    }
}
