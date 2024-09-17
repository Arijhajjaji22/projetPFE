using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_plateforme_de_recurtement.Models
{
    public class StageOffer
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
        public List<string> CompetencesRequises { get; set; }
        [Required]
        public DateTime DateDebut { get; set; }
        [Required]
        public DateTime DateFin { get; set; }
        [Required]
        public string NiveauEtudesRequis { get; set; }
        [Required]
        public string TypedeStage { get; set; }
        public bool Valide { get; set; }
        public bool IsDeleted { get; set; }
        public bool Desapprouve { get; set; }
        public bool Approved { get; set; }

     

        public DateTime DateDebutSansHeure => DateDebut.Date;
        public DateTime DateFinSansHeure => DateFin.Date;

    }
}