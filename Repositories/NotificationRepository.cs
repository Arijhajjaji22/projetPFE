using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Models;

namespace App_plateforme_de_recurtement.Repositories
{
    public class NotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Notification notification)
        {
            _context.notifications.Add(notification);
            _context.SaveChanges();
        }

        public List<Notification> GetAll()
        {
            return _context.notifications.ToList();
        }

        public List<Notification> GetForAdmin(int adminId)
        {
            // Logique pour récupérer les notifications pour un admin spécifique
            return _context.notifications.Where(n => n.UserId == adminId).ToList();
        }


    }
}
