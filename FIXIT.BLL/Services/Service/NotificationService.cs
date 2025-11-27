using AutoMapper;
using FIXIT.BLL.DTOs.NotificationDtos;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // يبعت الإشعار للحرفي فقط
            if (dto.CraftsManId.HasValue)
            {
                await _senderService.SendNotificationToUserAsync(
                    clientId: null,
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

            // يبعت الإشعار للعميل فقط
            if (dto.ClientId.HasValue)
            {
                await _senderService.SendNotificationToUserAsync(
                    clientId: dto.ClientId,
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
            return _mapper.Map<List<ReadNotificationDto>>(notifications
                .Where(n => n.SenderType == NotificationSenderType.Craftsman)); // بس اللي بعتها الحرفي
        }
        public async Task<List<ReadNotificationDto>> GetNotificationsForCraftsmanAsync(int craftsManId)
        {
            var notifications = await _repo.GetNotificationsForCraftsManAsync(craftsManId);
            return _mapper.Map<List<ReadNotificationDto>>(notifications
                .Where(n => n.SenderType == NotificationSenderType.Client)); // بس اللي بعتها العميل
        }

        //public async Task<List<ReadNotificationDto>> GetNotificationsForClientAsync(int clientId)
        //{
        //    var notifications = await _repo.GetNotificationsForClientAsync(clientId);
        //    return _mapper.Map<List<ReadNotificationDto>>(notifications);
        //}

        //public async Task<List<ReadNotificationDto>> GetNotificationsForCraftsmanAsync(int craftsManId)
        //{
        //    var notifications = await _repo.GetNotificationsForCraftsManAsync(craftsManId);
        //    return _mapper.Map<List<ReadNotificationDto>>(notifications);
        //}

        public async Task<ReadNotificationDto> MarkAsReadAsync(int id)
        {
            var notification = await _repo.GetByIdAsync(id);
            if (notification == null)
                return null;

            notification.IsRead = true;
            _repo.Update(notification, id);

            await _repo.SaveAsync();

            return _mapper.Map<ReadNotificationDto>(notification);
        }
    }
}
