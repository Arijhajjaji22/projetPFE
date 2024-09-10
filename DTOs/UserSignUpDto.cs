using System;
using System.ComponentModel.DataAnnotations;

namespace App_plateforme_de_recurtement.DTOs
{
    public class UserSignUpDto
    {
        [Required(ErrorMessage = "Le prénom est requis.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom de famille est requis.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "La date de naissance est requise.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
        public string Password { get; set; }
       // public string Role { get; set; }

        [Required(ErrorMessage = "La confirmation du mot de passe est requise.")]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        public string? ConfirmPassword { get; set; }

        // Propriétés pour la configuration SMTP
        // Propriétés SMTP (non requises lors de l'inscription)
        // Propriétés SMTP (facultatives)
        public string? SmtpServer { get; set; }
        public int? SmtpPort { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public string? SenderEmail { get; set; }
    }
}
