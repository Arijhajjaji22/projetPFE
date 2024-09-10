using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_plateforme_de_recurtement.Models
{
    public class RescheduleRequest
    {
        [Key]

        public int Id { get; set; } // Ajoutez un ID pour identifier chaque demande de reprogrammation
        public string NewDate { get; set; }
        public string NewTime { get; set; }
        public int? OfferId { get; set; }
        public int? TestId { get; set; }
        public int? UserId { get; set; }

        // Méthode pour obtenir la nouvelle date et heure combinées en DateTime
        public DateTime GetNewDateTime()
        {
            if (DateTime.TryParse($"{NewDate} {NewTime}", out var dateTime))
            {
                return dateTime;
            }
            throw new FormatException("La date ou l'heure spécifiée est invalide.");
        }
    }
}
