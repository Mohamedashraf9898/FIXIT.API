using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.SignalR;

namespace FIXIT.API.Hubs
{
    public class NotificationSenderService : INotificationSenderService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationSenderService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToUserAsync(int? clientId, int? craftsManId, string title,int? offerId, string message , NotificationSenderType senderType)
        {
            if (senderType == NotificationSenderType.Client && craftsManId.HasValue)
            {
                // لو العميل أرسل → الإشعار يروح للحرفي
                await _hubContext.Clients.User(craftsManId.Value.ToString())
                    .SendAsync("ReceiveNotification", title, message);
            }
            else if (senderType == NotificationSenderType.Craftsman && clientId.HasValue)
            {
                // لو الحرفي أرسل → الإشعار يروح للعميل
                await _hubContext.Clients.User(clientId.Value.ToString())
                    .SendAsync("ReceiveNotification", title, message);
            }
        }
    }
}
