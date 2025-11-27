using FIXIT.BLL.Services.IService;
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

        public async Task SendNotificationToUserAsync(int? clientId, int? craftsManId, string title, string message)
        {
            if (clientId.HasValue)
                await _hubContext.Clients.User(clientId.Value.ToString())
                    .SendAsync("ReceiveNotification", title, message);

            if (craftsManId.HasValue)
                await _hubContext.Clients.User(craftsManId.Value.ToString())
                    .SendAsync("ReceiveNotification", title, message);
        }
    }
}
