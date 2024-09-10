using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;

namespace App_plateforme_de_recurtement.Services
{
    public class NotificationService
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationService(NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public void CreateNotification(Notification notification)
        {
            _notificationRepository.Create(notification);
        }

        public List<Notification> GetAllNotifications()
        {
            return _notificationRepository.GetAll();
        }

        public List<Notification> GetNotificationsForAdmin(int adminId)
        {
            return _notificationRepository.GetForAdmin(adminId);
        }
    }

}
