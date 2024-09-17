using System;
using System.ComponentModel.DataAnnotations;
using App_plateforme_de_recurtement.Services;

namespace App_plateforme_de_recurtement.Models
{
    /*public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var userService = (UserService)validationContext
                .GetService(typeof(UserService)); // Obtenir le service à partir du contexte de validation

            var email = value as string;
            if (email != null)
            {
                // Utiliser le service UserService pour vérifier l'unicité de l'e-mail
                if (!userService.IsEmailUnique(email))
                {
                    return new ValidationResult("L'e-mail est déjà utilisé.");
                }
            }

            return ValidationResult.Success;
        }
    }*/
}
