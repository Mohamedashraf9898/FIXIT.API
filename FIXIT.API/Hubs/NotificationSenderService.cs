using FIXIT.BLL.DTOs.NotificationDtos;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using FIXIT.API.Hubs;
using FIXIT.DAL.Models.Identity;

namespace FIXIT.BLL.Services.Service
{
    public class NotificationSenderService : INotificationSenderService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationRepository _notificationRepo;
        private readonly IClientRepo _clientRepo;
        private readonly ICraftsManRepo _craftsManRepo;
        private readonly IMapper _mapper;

        public NotificationSenderService(
            IHubContext<NotificationHub> hubContext,
            INotificationRepository notificationRepo,
            IClientRepo clientRepo,
            ICraftsManRepo craftsManRepo,
            IMapper mapper)
        {
            _hubContext = hubContext;
            _notificationRepo = notificationRepo;
            _clientRepo = clientRepo;
            _craftsManRepo = craftsManRepo;
            _mapper = mapper;
        }

        public async Task SendNotificationToUserAsync(
      int? clientId,
      int? offerId,
      int? craftsManId,
      string title,
      string message,
      NotificationSenderType senderType)
        {
            try
            {
                string? recipientId = null;

                if (senderType == NotificationSenderType.Client && craftsManId.HasValue)
                {
                    
                    recipientId = $"craftsman_{craftsManId.Value}";  
                    Console.WriteLine($"📤 Client → Craftsman (SignalR ID: {recipientId})");
                }
                else if (senderType == NotificationSenderType.Craftsman && clientId.HasValue)
                {
              
                    recipientId = $"client_{clientId.Value}";  
                    Console.WriteLine($"📤 Craftsman → Client (SignalR ID: {recipientId})");
                }

                if (string.IsNullOrEmpty(recipientId))
                {
                    Console.WriteLine("⚠️ No recipient ID");
                    return;
                }

                // Get notifications from database
                IEnumerable<Notification> notifications;

                if (senderType == NotificationSenderType.Craftsman && clientId.HasValue)
                {
                    notifications = await _notificationRepo.GetNotificationsForClientAsync(clientId.Value);
                }
                else if (senderType == NotificationSenderType.Client && craftsManId.HasValue)
                {
                    notifications = await _notificationRepo.GetNotificationsForCraftsManAsync(craftsManId.Value);
                }
                else
                {
                    Console.WriteLine("⚠️ Invalid parameters");
                    return;
                }

                var latestNotification = notifications.OrderByDescending(n => n.CreatedAt).FirstOrDefault();

                if (latestNotification == null)
                {
                    Console.WriteLine("⚠️ Notification not found in DB");
                    return;
                }

                // Map to DTO
                var notificationDto = _mapper.Map<ReadNotificationDto>(latestNotification);

                Console.WriteLine($"🚀 Sending SignalR to: {recipientId}");
                Console.WriteLine($"   Notification: ID={latestNotification.Id}, Type={latestNotification.Type}");

                // ✅ Send to prefixed user ID
                await _hubContext.Clients.User(recipientId)
                    .SendAsync("NotificationReceived", notificationDto);

                Console.WriteLine($"✅ Notification sent to {recipientId}!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"   Stack: {ex.StackTrace}");
            }
        }
      
        public async Task SendNotificationToAdminAsync(Notification notification)
        {
            try
            {
                var notificationDto = _mapper.Map<ReadNotificationDto>(notification);

                
                await _hubContext.Clients.Group("Admin").SendAsync("NotificationReceived", notificationDto);

                Console.WriteLine($"✅ Notification sent to Admin group only!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
        public async Task SendNotificationToCraftsmanFromAdminAsync(Notification notification, int craftsManId)
        {
            try
            {
                string recipientId = $"craftsman_{craftsManId}";

                Console.WriteLine($"📤 Admin → Craftsman (SignalR ID: {recipientId})");

                var notificationDto = _mapper.Map<ReadNotificationDto>(notification);

                await _hubContext.Clients.User(recipientId)
                    .SendAsync("NotificationReceived", notificationDto);

                Console.WriteLine($"✅ Notification sent to craftsman {recipientId}!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }
}