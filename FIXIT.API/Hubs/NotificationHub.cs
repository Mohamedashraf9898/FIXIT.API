using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FIXIT.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var clientId = Context.User?.FindFirst("clientId")?.Value;
            var craftsmanId = Context.User?.FindFirst("craftsmanId")?.Value;

            var userId = role == "Client"
                ? $"client_{clientId}"
                : $"craftsman_{craftsmanId}";

            Console.WriteLine($"✅ {role} connected to NotificationHub:");
            Console.WriteLine($"   - SignalR User ID: {userId}");
            Console.WriteLine($"   - Connection ID: {Context.ConnectionId}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var clientId = Context.User?.FindFirst("clientId")?.Value;
            var craftsmanId = Context.User?.FindFirst("craftsmanId")?.Value;

            var userId = role == "Client"
                ? $"client_{clientId}"
                : $"craftsman_{craftsmanId}";

            Console.WriteLine($"❌ {role} {userId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        // ✅ Custom UserIdProvider - Uses clientId or craftsmanId from JWT
        public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var role = connection.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Client")
            {
                var clientId = connection.User?.FindFirst("clientId")?.Value;
                if (!string.IsNullOrEmpty(clientId))
                {
                    return $"client_{clientId}";  // ✅ Prefix with "client_"
                }
            }
            else if (role == "CraftsMan")
            {
                var craftsmanId = connection.User?.FindFirst("craftsmanId")?.Value;
                if (!string.IsNullOrEmpty(craftsmanId))
                {
                    return $"craftsman_{craftsmanId}";  // ✅ Prefix with "craftsman_"
                }
            }

            return null;
        }
    }
}
}