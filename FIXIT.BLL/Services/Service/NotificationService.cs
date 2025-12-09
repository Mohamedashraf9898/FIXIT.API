using AutoMapper;
using FIXIT.BLL.DTOs.NotificationDtos;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;



namespace FIXIT.BLL.Services.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IMapper _mapper;
        private readonly INotificationSenderService _senderService;

        public NotificationService(INotificationRepository repo, IMapper mapper, INotificationSenderService senderService)
        {
            _repo = repo;
            _mapper = mapper;
            _senderService = senderService;
        }

        public async Task CreateFromClientAsync(CreateNotificationDto dto)
        {
            var notification = _mapper.Map<Notification>(dto);
            notification.SenderType = NotificationSenderType.Client;

            await _repo.AddAsync(notification);
            await _repo.SaveAsync();

            if (dto.CraftsManId.HasValue)
            {
                await _senderService.SendNotificationToUserAsync(
                    clientId: null,
                    offerId: dto.OfferId,
                    craftsManId: dto.CraftsManId,
                    title: dto.Title,
                    message: dto.Message,
                    senderType: NotificationSenderType.Client
                );
            }
        }

        public async Task CreateFromCraftsmanAsync(CreateNotificationDto dto)
        {
            var notification = _mapper.Map<Notification>(dto);
            notification.SenderType = NotificationSenderType.Craftsman;

            await _repo.AddAsync(notification);
            await _repo.SaveAsync();

            if (dto.ClientId.HasValue)
            {
                await _senderService.SendNotificationToUserAsync(
                    clientId: dto.ClientId,
                    offerId: dto.OfferId,
                    craftsManId: null,
                    title: dto.Title,
                    message: dto.Message,
                    senderType: NotificationSenderType.Craftsman
                );
            }
        }

        public async Task<List<ReadNotificationDto>> GetNotificationsForClientAsync(int clientId)
        {
            var notifications = await _repo.GetNotificationsForClientAsync(clientId);
            var filtered = notifications
                .Where(n => n.SenderType == NotificationSenderType.Craftsman)
                .ToList();

            return _mapper.Map<List<ReadNotificationDto>>(filtered);
        }

        public async Task<List<ReadNotificationDto>> GetNotificationsForCraftsmanAsync(int craftsManId)
        {
            var notifications = await _repo.GetNotificationsForCraftsManAsync(craftsManId);
            var filtered = notifications
                .Where(n => n.SenderType == NotificationSenderType.Client ||
                           n.SenderType == NotificationSenderType.Admin)  // ✅ Craftsman can receive from Admin too
                .ToList();

            return _mapper.Map<List<ReadNotificationDto>>(filtered);
        }

        public async Task<ReadNotificationDto> MarkAsReadAsync(int id)
        {
            var notification = await _repo.GetByIdAsync(id);
            if (notification == null)
                return null;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _repo.Update(notification, id);
                await _repo.SaveAsync();
            }

            return _mapper.Map<ReadNotificationDto>(notification);
        }

        public async Task<ReadNotificationDto> CreateForAdminAsync(CreateNotificationDto dto)
        {
            var notification = _mapper.Map<Notification>(dto);
            
            notification.CreatedAt = DateTime.Now;
            notification.IsRead = false;

            await _repo.AddAsync(notification);
            await _repo.SaveAsync();

            // ✅ Send via SignalR to Admin
            await _senderService.SendNotificationToAdminAsync(notification);

            var result = _mapper.Map<ReadNotificationDto>(notification);
            return result;
        }

        public async Task CreateFromAdminToCraftsmanAsync(CreateNotificationDto dto)
        {
            var notification = _mapper.Map<Notification>(dto);
            notification.SenderType = NotificationSenderType.Admin;
            notification.IsRead = false;
            notification.CreatedAt = DateTime.UtcNow;

            await _repo.AddAsync(notification);
            await _repo.SaveAsync();

            if (dto.CraftsManId.HasValue)
            {
                await _senderService.SendNotificationToCraftsmanFromAdminAsync(
                    notification,
                    dto.CraftsManId.Value
                );
            }
        }

        
        public async Task<List<ReadNotificationDto>> GetNotificationsForAdminAsync()
        {
            var notifications = await _repo.GetNotificationsForAdminAsync();
            return _mapper.Map<List<ReadNotificationDto>>(notifications);
        }
        public async Task SendCancellationNotificationsAsync(int serviceRequestId, int craftsManId, int clientId, string reasonType, string clientName, string serviceName)
        {
            // Notification for Craftsman
            var craftsmanNotification = new CreateNotificationDto
            {
                ServiceRequestId = serviceRequestId,
                CraftsManId = craftsManId,
                ClientId = clientId,
                Title = "Service Cancellation",
                Message = reasonType == "craftsman_no_show"
                    ? $"{clientName} has cancelled the {serviceName} service because you did not show up."
                    : $"{clientName} has cancelled the {serviceName} service.",
                SenderType = NotificationSenderType.Client,
                Type = reasonType == "craftsman_no_show"
                    ? NotificationType.CraftsmanNoShow
                    : NotificationType.ServiceCancelled,
                IsRead = false
            };

            await CreateFromClientAsync(craftsmanNotification);

            // Notification for Admin
            var adminNotification = new CreateNotificationDto
            {
                ServiceRequestId = serviceRequestId,
                CraftsManId = craftsManId,
                ClientId = clientId,
                Title = "Refund Required",
                Message = $"Client {clientName} has cancelled service request #{serviceRequestId} ({serviceName}). Reason: {(reasonType == "craftsman_no_show" ? "Craftsman no-show" : "Client request")}. Please process refund.",
                SenderType = NotificationSenderType.Client,
                Type = NotificationType.ServiceCancelled,
                IsRead = false
            };

            await CreateForAdminAsync(adminNotification);
        }
    }
}