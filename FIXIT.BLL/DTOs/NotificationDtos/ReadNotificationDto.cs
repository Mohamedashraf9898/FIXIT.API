using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.DTOs.NotificationDtos
{
    public class ReadNotificationDto
    {
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public int? OfferId { get; set; }
        public int ServiceRequestId { get; set; }
        public string Title { get; set; }
        public decimal? FinalAmount { get; set; }
        public string? Description { get; set; }
         
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ClientName { get; set; }     
        public string? CraftsManName { get; set; }
    }
}
