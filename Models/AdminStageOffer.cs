using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_plateforme_de_recurtement.Models
{
    public class AdminStageOffer
    {
        [Key]
        public int Id { get; set; }
       

        
       


        [Required]
        public string Titre { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string DomaineActivite { get; set; }

        [Required]
        public string CompetencesRequises { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public DateTime DateFin { get; set; }

        [Required]
        public string NiveauEtudesRequis { get; set; }

        [Required]
        public string TypedeStage { get; set; }
        public bool Desapprouve { get; set; }




        public bool Approved { get; set; }

        // ID de l'utilisateur qui a validé l'offre
       // public string ValidatedByUserId { get; set; }

        public DateTime DateDebutSansHeure => DateDebut.Date;

        public DateTime DateFinSansHeure => DateFin.Date;
        public ICollection<Form> Forms { get; set; }


    }
}
