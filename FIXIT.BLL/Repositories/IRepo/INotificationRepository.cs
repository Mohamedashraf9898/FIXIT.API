using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsForClientAsync(int clientId);
        Task<IEnumerable<Notification>> GetNotificationsForCraftsManAsync(int craftsManId);
        Task SaveAsync();
        Task<Notification?> GetByIdAsync(int id);
        void Update(Notification notification, int id);
        Task<IEnumerable<Notification>> GetNotificationsForAdminAsync();
    }
}
