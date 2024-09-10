using System.ComponentModel.DataAnnotations;

namespace App_plateforme_de_recurtement.Models
{
    public class Competence
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nom { get; set; }
    }
}
