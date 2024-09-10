using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace App_plateforme_de_recurtement.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
        public string Username { get; set; }
        public string? FullName { get; set; }
        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Le rôle est requis.")]
       
        public string Role { get; set; } 

        [Required]
        public bool IsArchived { get; set; }
        // Attributs supplémentaires pour les candidats
        public string? ValidationToken { get; set; }
        public string? FirstName { get; set; }
        
  
        public string? LastName { get; set; }


        public DateTime? DateOfBirth { get; set; }
  
    public bool IsEmailVerified { get; set; }
        public User()
        {
            FirstName = "Unknown";   // Valeur par défaut pour FirstName
            LastName = "Unknown";
        }
    
    }
}
