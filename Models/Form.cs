using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_plateforme_de_recurtement.Models
{
    public class Form
    {
        [Key]
        public int Id { get; set; }

        public string? Prenom { get; set; }
   public string? Nom { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

       
        public string? Telephone { get; set; }

  
        public string? Faculte { get; set; }

        public string? NiveauEtudes { get; set; }

        public byte[]? CVData { get; set; } // Stocke les données binaires du CV

        public byte[]? LettreMotivationData { get; set; } // Stocke les données binaires de la lettre de motivation

        // Nouvelle propriété pour l'identifiant de AdminStageOffers
        public int? AdminStageOfferId { get; set; }
        //propriété de navigation
        public AdminStageOffer? AdminStageOffer { get; set; }
        // Nouvelle propriété pour l'identifiant de Test

        // Propriété de navigation
      //  [ForeignKey("TestId")]
      //  public Test? Test { get; set; }
        // Nouvelle propriété pour l'identifiant de Test
        // Nouvelle propriété pour l'identifiant de Test
        //     public int? TestId { get; set; }
        // public Test Test { get; set; }

        // Ajout d'une propriété pour stocker le rapport PDF
        public byte[]? PdfReportData { get; set; }
        // Ajout d'une propriété UserId pour le lien avec l'utilisateur
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Report { get; set; }
        // Nouvelle propriété pour l'identifiant de Test
      //  public int? TestId { get; set; }
     
        public string? Status { get; set; } // Nouveau champ pour l'état du formulaire
        public int? StarRating { get; set; }
        public bool? InterviewPassed { get; set; }
        public DateTime? ScheduledDate { get; set; }
     
    }
}
