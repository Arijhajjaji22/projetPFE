using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_plateforme_de_recurtement.Models
{
    public class RescheduleRequesttest
    {
        [Key]
        public int Id { get; set; } // Ajoutez un ID pour identifier chaque demande de reprogrammation
        public DateTime? NewDateTime { get; set; }
        public string  Subject { get; set; } // Remplacez OfferId par SubjectId
        public int TestId { get; set; } = 0; // ou une autre valeur par défaut appropriée
        public int? UserId { get; set; }
    }
}
