using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.SchedulingDTOs
{
    public class CreateTimeOffDto
    {
        [Required]
        public int CraftsManId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public TimeOffType Type { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
