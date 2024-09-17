using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.DTOs;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Services;
using Microsoft.AspNetCore.Mvc;

namespace App_plateforme_de_recurtement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserService _userService;
        private readonly ILogger<UsersController> _logger;
       



        public UsersController(UserService userService, ApplicationDbContext applicationDbContext, ILogger<UsersController> logger)
        {
            _userService = userService;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
           

        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            // var users = _userService.GetAllUsers().Where(u=>u.IsArchived=false);
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpPost]
       
        public IActionResult CreateUser([FromBody] User user)
        {
            try
            {
                if (user == null)
                    return BadRequest("User object is null");

                if (!ModelState.IsValid)
                    return BadRequest("Invalid model object");

                if (_userService.IsEmailAlreadyExists(user.Email))
                    return BadRequest("Cet email existe déjà. Veuillez saisir un autre email.");

               
                if (user.Password != user.ConfirmPassword)
                    return BadRequest("Les mots de passe ne correspondent pas.");

                _userService.AddUser(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la création de l'utilisateur.");
                return StatusCode(500, "Une erreur interne s'est produite. Veuillez réessayer plus tard.");
            }
        }




        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest("L'ID dans le chemin d'accès ne correspond pas à l'ID de l'utilisateur fourni dans le corps de la requête.");
            }

            var existingUser = _userService.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Vérifier si l'e-mail a changé avant de valider s'il existe déjà
            if (existingUser.Email != user.Email && _userService.IsEmailAlreadyExists(user.Email))
            {
                return BadRequest("Cet email existe déjà. Veuillez saisir un autre email.");
            }

            // Mettre à jour les propriétés de l'utilisateur existant avec les nouvelles valeurs
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            existingUser.Role = user.Role;

            _userService.UpdateUser(existingUser); // Appel à la méthode de service pour mettre à jour l'utilisateur
            return NoContent();
        }










        /* [HttpDelete("/api/Users/{userId}")]
         public IActionResult ArchiveUser(int id)
         {
             _userService.ArchiveUser(id);
             return NoContent();
         }
     }*/
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound(); // Utilisateur non trouvé
            }

            user.IsArchived = true; // Mettre à jour la propriété IsArchived à true

            _userService.UpdateUser(user); // Mettre à jour l'utilisateur dans la base de données

            return NoContent(); // Succès, retourner aucune donnée
        }

        [HttpGet("info/{email}")]
        public async Task<IActionResult> GetUserInfoByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound("Utilisateur non trouvé.");

                var userInfo = new
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username
                };

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur interne s'est produite : {ex.Message}");
            }
        }
        }

}
