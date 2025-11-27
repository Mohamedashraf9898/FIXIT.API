using FIXIT.BLL.DTOs.NotificationDtos;
using FIXIT.BLL.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface INotificationService 
    {
        Task CreateFromClientAsync(CreateNotificationDto dto);
        Task CreateFromCraftsmanAsync(CreateNotificationDto dto);

        Task<List<ReadNotificationDto>> GetNotificationsForClientAsync(int clientId);
        Task<List<ReadNotificationDto>> GetNotificationsForCraftsmanAsync(int craftsManId);
        Task<ReadNotificationDto> MarkAsReadAsync(int id);
    }
}
