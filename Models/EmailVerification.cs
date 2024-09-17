using System.ComponentModel.DataAnnotations;

namespace App_plateforme_de_recurtement.Models
{
    public class EmailVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
