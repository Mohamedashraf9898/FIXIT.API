using Microsoft.AspNetCore.SignalR;

namespace FIXIT.API.Hubs
{
    public class NotificationHub : Hub
    {

        public override async Task OnConnectedAsync()
        {

            await base.OnConnectedAsync();
        }

        public async Task SendMessageToServer(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
