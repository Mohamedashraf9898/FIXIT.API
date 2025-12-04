using FIXIT.DAL.Models;

namespace FIXIT.BLL.Services.IService
{
    public interface INotificationSenderService
    {
        Task SendNotificationToUserAsync(
            int? clientId,
            int? offerId,
            int? craftsManId,
            string title,
            string message,
            NotificationSenderType senderType);

        Task SendNotificationToAdminAsync(Notification notification);

        Task SendNotificationToCraftsmanFromAdminAsync(Notification notification, int craftsManId);
    }
}