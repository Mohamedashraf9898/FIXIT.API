using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.NotificationDtos
{
    public class CreateNotificationDto
    {
     
        public int ServiceRequestId { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public int? ClientId { get; set; }     
        public int? CraftsManId { get; set; }
        public NotificationSenderType SenderType { get; set; }

        public NotificationType Type { get; set; }
    }
}
