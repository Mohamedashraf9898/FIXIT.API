using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.Repo
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly FixItDbContext _dbContext;

        public NotificationRepository(FixItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Notification notification)
        {
            await _dbContext.Notifications.AddAsync(notification);
        }

        

       
       
        
        public void Update(Notification notification, int id)
        {
            _dbContext.Notifications.Update(notification);
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Notification>> GetNotificationsForClientAsync(int clientId)
        {
            return await _dbContext.Notifications
                .Include(n => n.ServiceRequest)
                    .ThenInclude(sr => sr.CraftsMan)
                .Include(n => n.Offer)
                .Where(n => n.ServiceRequest.ClientId == clientId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsForCraftsManAsync(int craftsManId)
        {
            return await _dbContext.Notifications
                .Include(n => n.ServiceRequest)
                    .ThenInclude(sr => sr.CraftsMan)
                .Include(n => n.Offer)
                .Where(n => n.ServiceRequest.CraftsManId == craftsManId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _dbContext.Notifications
                .Include(n => n.ServiceRequest)
                    .ThenInclude(sr => sr.CraftsMan)
                .Include(n => n.Offer)
                .FirstOrDefaultAsync(n => n.Id == id);
        }
        public async Task<IEnumerable<Notification>> GetNotificationsForAdminAsync()
        {
            return await _dbContext.Notifications
                .Where(n =>
                    (n.SenderType == NotificationSenderType.Craftsman && n.Type == NotificationType.WithdrawalRequested) ||
                    n.Type == NotificationType.ServiceCancelled ||
                    n.Type == NotificationType.CraftsmanNoShow)
                .Include(n => n.ServiceRequest)
                .Include(n => n.Offer)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
