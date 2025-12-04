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
            var adminId = Context.User?.FindFirst("adminId")?.Value;  // ✅ Add this

            var userId = role == "Client" ? $"client_{clientId}"
                       : role == "CraftsMan" ? $"craftsman_{craftsmanId}"
                       : role == "Admin" ? $"admin_{adminId}"  // ✅ Add this
                       : null;

            Console.WriteLine($"✅ {role} connected to NotificationHub:");
            Console.WriteLine($"   - SignalR User ID: {userId}");
            Console.WriteLine($"   - Connection ID: {Context.ConnectionId}");
            if (role == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
                Console.WriteLine($"✅ Admin added to Admin group");
            }

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
            var adminId = Context.User?.FindFirst("adminId")?.Value;  // ✅ Add this

            var userId = role == "Client" ? $"client_{clientId}"
                       : role == "CraftsMan" ? $"craftsman_{craftsmanId}"
                       : role == "Admin" ? $"admin_{adminId}"  // ✅ Add this
                       : null;

            Console.WriteLine($"❌ {role} {userId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        public class CustomUserIdProvider : IUserIdProvider
        {
            public string? GetUserId(HubConnectionContext connection)
            {
                var role = connection.User?.FindFirst(ClaimTypes.Role)?.Value;

                if (role == "Client")
                {
                    var clientId = connection.User?.FindFirst("clientId")?.Value;
                    if (!string.IsNullOrEmpty(clientId))
                        return $"client_{clientId}";
                }
                else if (role == "CraftsMan")
                {
                    var craftsmanId = connection.User?.FindFirst("craftsmanId")?.Value;
                    if (!string.IsNullOrEmpty(craftsmanId))
                        return $"craftsman_{craftsmanId}";
                }
                else if (role == "Admin")  // ✅ Add this
                {
                    var adminId = connection.User?.FindFirst("adminId")?.Value;
                    if (!string.IsNullOrEmpty(adminId))
                        return $"admin_{adminId}";
                }

                return null;
            }
        }
    }
}
