using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface INotificationSenderService
    {
        Task SendNotificationToUserAsync(int? clientId, int? craftsManId, string title,int? offerId ,string message, NotificationSenderType senderType);

    }
}
