using System.ComponentModel.DataAnnotations.Schema;

namespace App_plateforme_de_recurtement.Models
{
    public class AvailabilityConfirmation
    {
        public int Id { get; set; }

        [ForeignKey("Availability")]
        public int AvailabilityId { get; set; }
        public bool IsConfirmed { get; set; }

        public Availability Availability { get; set; }
        public int? UserId { get; set; } // Ajouter le champ UserId
    }
}
