using App_plateforme_de_recurtement.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using App_plateforme_de_recurtement.DTOs;
using App_plateforme_de_recurtement.Services;
using App_plateforme_de_recurtement.Repositories;
using Newtonsoft.Json.Linq;
using App_plateforme_de_recurtement.Models;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace App_plateforme_de_recurtement.Controllers
{

    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserService _userService;
        private readonly UserRepository _userRepository;
        private readonly EmailService _emailService;

        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext applicationDbContext, UserService userService, EmailService emailService, ApplicationDbContext context)
        {
            _applicationDbContext = applicationDbContext;
            _userService = userService;
            _emailService = emailService;
            _context = context;
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            Console.WriteLine("Email: " + email);
            Console.WriteLine("Password: " + password);

            // Vérifie si l'email ou le mot de passe est vide
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = "Veuillez saisir un email et un mot de passe." });
            }

            // Recherche l'utilisateur dans la base de données
            var user = _applicationDbContext.Users.SingleOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Récupérer l'ID de l'utilisateur connecté
                int userId = user.Id;
                // Afficher l'ID de l'utilisateur dans la console
                Console.WriteLine("User ID: " + userId);
                // Retourner le rôle de l'utilisateur
                return Json(new
                {
                    success = true,
                    role = user.Role,
                    userId = user.Id // Envoyer l'ID de l'utilisateur au front-end
                });
            }
            else
            {
                // Authentification échouée, retourner un message d'erreur
                return Json(new { success = false, message = "Email ou mot de passe incorrect." });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto userLoginDto)
        {
            if (userLoginDto == null)
            {
                userLoginDto = new UserLoginDto
                {
                    Email = Request.Form["email"],
                    Password = Request.Form["password"]
                };
            }

            Console.WriteLine("Email: " + userLoginDto.Email);
            Console.WriteLine("Password: " + userLoginDto.Password);

            if (string.IsNullOrEmpty(userLoginDto.Email) || string.IsNullOrEmpty(userLoginDto.Password))
            {
                return BadRequest(new { success = false, message = "Veuillez saisir un email et un mot de passe." });
            }

            // Utiliser AuthenticateUser pour vérifier les informations de connexion
            var token = _userService.AuthenticateUser(userLoginDto.Email, userLoginDto.Password);

            if (token != null)
            {
                var user = _userRepository.GetUserByEmail(userLoginDto.Email);

                if (user != null)
                {
                    // Renvoyer les informations utilisateur avec le token
                    return Ok(new
                    {
                        success = true,
                        role = user.Role,
                        userId = user.Id,
                        userDetails = new
                        {
                            prenom = user.FirstName,
                            nom = user.LastName,
                            email = user.Email
                        }
                    });
                }
                else
                {
                    return NotFound(new { success = false, message = "Utilisateur non trouvé." });
                }
            }
            else
            {
                return Unauthorized(new { success = false, message = "Email ou mot de passe incorrect." });
            }
        }
        [HttpGet("signin-google")]
        public async Task<IActionResult> SignInGoogle()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return BadRequest();

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
                return BadRequest();

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FirstName = authenticateResult.Principal.FindFirst(ClaimTypes.GivenName)?.Value,
                    LastName = authenticateResult.Principal.FindFirst(ClaimTypes.Surname)?.Value,
                    Role = "User", // Par défaut ou selon votre logique
                    IsEmailVerified = true // Considéré comme vérifié via Google
                };

                await _userService.AddUserAsync(user);
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticateResult.Principal);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult SignIn()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleSignInCallback") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet("google-signin-callback")]
        public async Task<IActionResult> GoogleSignInCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return BadRequest(); // Gérer le scénario d'erreur

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var emailClaim = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
                return BadRequest("L'e-mail n'a pas été fourni par Google ou est vide.");

            var email = emailClaim.Value;
            var firstName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
            var picture = claims?.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;

            // Vérifiez si l'utilisateur existe déjà dans la base de données
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                // Créez un nouvel utilisateur si non existant
                user = new User
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Role = "User", // Par défaut ou selon votre logique
                    IsEmailVerified = true // Considéré comme vérifié via Google
                };
                await _userRepository.AddUserAsync(user);
            }

            // Connectez l'utilisateur en utilisant une identité personnalisée
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.GivenName, user.FirstName),
        new Claim(ClaimTypes.Surname, user.LastName),
        new Claim("urn:google:picture", picture ?? string.Empty)
    }, CookieAuthenticationDefaults.AuthenticationScheme));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

            return RedirectToAction("Index", "Home"); // Rediriger vers la page d'accueil ou une autre page
        }





        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpDto userSignUpDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }

            try
            {
                await _userService.RegisterUser(userSignUpDto);
                return Ok(new { Message = "Un email de validation a été envoyé." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        // Méthode pour mettre à jour la configuration SMTP
        private void UpdateSmtpConfiguration(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, string senderEmail)
        {
            var configPath = "appsettings.json";
            var configJson = System.IO.File.ReadAllText(configPath);
            var configObj = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(configJson);

            configObj["SmtpConfig"]["SmtpServer"] = smtpServer;
            configObj["SmtpConfig"]["SmtpPort"] = smtpPort.ToString();
            configObj["SmtpConfig"]["SmtpUsername"] = smtpUsername;
            configObj["SmtpConfig"]["SmtpPassword"] = smtpPassword;
            configObj["SmtpConfig"]["SenderEmail"] = senderEmail;

            var updatedConfigJson = Newtonsoft.Json.JsonConvert.SerializeObject(configObj, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(configPath, updatedConfigJson);
        }
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                var result = await _userService.VerifyEmail(token);
                if (result)
                {
                    return RedirectToAction("EmailVerified", "Home");
                }
                else
                {
                    return BadRequest("Erreur lors de la vérification de l'email.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la vérification de l'email : {ex.Message}");
            }
        }

        private bool IsEmailVerified(string email)
        {
            // Recherchez l'utilisateur dans la base de données par e-mail
            var user = _applicationDbContext.Users.SingleOrDefault(u => u.Email == email);

            // Vérifiez si l'utilisateur existe et si son e-mail est vérifié
            return user != null && user.IsEmailVerified;
        }




        public async Task<IActionResult> Logout()
        {
            // Effectuer les opérations de déconnexion, telles que la suppression des cookies d'authentification

            // Rediriger vers la page spécifiée après la déconnexion
            return RedirectToAction("SeConnecter", "Home");
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendEmail(string recipientEmail, string subject, string body)
        {
            try
            {
                await _emailService.SendEmail(recipientEmail, subject, body);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }

    }
}