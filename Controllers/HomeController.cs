using App_plateforme_de_recurtement.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using App_plateforme_de_recurtement.DTOs;
using App_plateforme_de_recurtement.Repositories;
using System.Security.Claims;

namespace App_plateforme_de_recurtement.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _userRepository;
        public HomeController(UserRepository userRepository)
        {
            _userRepository = userRepository;
                            
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ChatHub()
        {
            return View();
        }

        public IActionResult about()
        {
            return View();
        }
        public IActionResult Tasks()
        {
            return View();
        }
        public IActionResult details()
        {
            return View();
        }
        public IActionResult contact()
        {
            return View();
        }
        public IActionResult faq()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }
        public IActionResult GestionUtilisateurs()
        {
            return View();
        }

        public IActionResult SeConnecter()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult ManagerDashboard()
        {
            return View();
        }
        public IActionResult Gestiontasks()
        {
            return View();
        }
        public IActionResult RhDashboard()
        {
            return View();
        }
     
     public IActionResult GestionSujets()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Tests()
        {
            return View();
        }
        public IActionResult Creationtests()
        {
            return View();
        }
        public IActionResult Testform()
        {
            return View();
        }
        public IActionResult formcandidat()
        {
            return View();
        }
        public IActionResult detailssujet()
        {
            return View();
        }
        public IActionResult testdetails()
        {
            return View();
        }
        public IActionResult testdetailsmail()
        {
            return View();
        }
        public IActionResult test()
        {
            return View();
        }
        public IActionResult testsucess()
        {
            return View();
        }
        public IActionResult DashboardRh()
        {
            return View();
        }
        public IActionResult interfacecandidat()
        {
            return View();
        }
        public IActionResult gestionentretiens()
        {
            return View();
        }
       public IActionResult Calendriercandidat()
        {
            return View();
        }
        public IActionResult SeConnectercandidat()
        {
            return View();
        }
        public IActionResult Gestioncandidat()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult SignUpcandidat()
        {
            return View();
        }
        public IActionResult tablesujets()
        {
            return View();
        }
        public IActionResult tablesujetsrh()
        {
            return View();
        }

        public IActionResult Gestioncandidatrh()
        {
            return View();
        }
        public IActionResult EmailVerified(string email)
        {
            var model = new EmailVerification
            {
                Email = email,
                CreatedAt = DateTime.Now,
                // Assurez-vous de remplir les autres propriétés nécessaires comme `Token`
            };
            return View(model);
        }
        [AllowAnonymous]
        [Route("Account/GoogleAuth")]
        public IActionResult GoogleAuth()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [Route("Account/GoogleResponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            var offerId = Request.Cookies["offerId"]; // Récupérer l'ID de l'offre depuis le cookie

            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return BadRequest(); // Gérer l'erreur de manière appropriée
            }

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name); // Récupérer le nom complet de l'utilisateur

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("L'email n'a pas été trouvé dans les informations d'authentification Google.");
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(email);

            if (existingUser == null)
            {
                // Générez un mot de passe aléatoire
                var password = GenerateRandomPassword();

                // Créez un nouvel utilisateur avec email, mot de passe, nom d'utilisateur et nom complet
                var newUser = new User
                {
                    Email = email,
                    Password = password, // Enregistrez le mot de passe généré aléatoirement
                    Username = GenerateUsernameFromEmail(email), // Générez un nom d'utilisateur à partir de l'email, ou utilisez une autre logique
                    FullName = fullName, // Enregistrez le nom complet récupéré
                    Role = "User", // Définissez le rôle de l'utilisateur selon votre logique
                    IsEmailVerified = true // Assurez-vous de gérer la vérification d'email si nécessaire
                };

                await _userRepository.AddUserAsync(newUser);
            }

            // Redirigez l'utilisateur vers une page appropriée après l'authentification réussie
            return RedirectToAction("formcandidat", "Home", new { id = offerId });
        }



        // Méthode pour générer un nom d'utilisateur à partir de l'email
        private string GenerateUsernameFromEmail(string email)
        {
            // Exemple : Utiliser une partie de l'email comme nom d'utilisateur
            var parts = email.Split('@');
            return parts[0]; // Utilisez la première partie de l'email comme nom d'utilisateur
        }

        // Méthode pour générer un mot de passe aléatoire
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }




    }
}
