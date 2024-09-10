using System.ComponentModel.DataAnnotations;

namespace App_plateforme_de_recurtement.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        public string Password { get; set; }
    }
}
