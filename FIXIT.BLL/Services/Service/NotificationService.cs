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

        public async Task CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notification = _mapper.Map<Notification>(dto);
            await _repo.AddAsync(notification);
            await _repo.SaveAsync();

            
            await _senderService.SendNotificationToUserAsync(dto.ClientId, dto.CraftsManId, dto.Title, dto.Message);
        }

        public async Task<List<ReadNotificationDto>> GetNotificationsForClientAsync(int clientId)
        {
            var notifications = await _repo.GetNotificationsForClientAsync(clientId);
            return _mapper.Map<List<ReadNotificationDto>>(notifications);
        }

        public async Task<List<ReadNotificationDto>> GetNotificationsForCraftsmanAsync(int craftsManId)
        {
            var notifications = await _repo.GetNotificationsForCraftsManAsync(craftsManId);
            return _mapper.Map<List<ReadNotificationDto>>(notifications);
        }

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
