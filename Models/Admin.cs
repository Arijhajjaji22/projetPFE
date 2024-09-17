using System.ComponentModel.DataAnnotations;

namespace App_plateforme_de_recurtement.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
