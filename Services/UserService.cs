using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.DTOs;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace App_plateforme_de_recurtement.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public UserService(UserRepository userRepository, ApplicationDbContext context, EmailService emailService)
        {
            _userRepository = userRepository;
            _context = context;
            _emailService = emailService;
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public User GetUserById(int id)
        {
            return _userRepository.GetUserById(id);
        }

        public void AddUser(User user)
        {
            _userRepository.AddUser(user);
        }

        public void UpdateUser(User user)
        {
            _userRepository.UpdateUser(user);
        }

        public void DeleteUser(int id)
        {
            _userRepository.DeleteUser(id);
        }

        public bool IsEmailAlreadyExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public async Task<int> RegisterUser(UserSignUpDto userSignUpDto)
        {
            if (IsEmailAlreadyExists(userSignUpDto.Email))
            {
                throw new Exception("Email already exists.");
            }

            if (userSignUpDto.Password != userSignUpDto.ConfirmPassword)
            {
                throw new Exception("Passwords do not match.");
            }

            var validationToken = Guid.NewGuid().ToString();

            var tempUser = new UserRegistrationTemp
            {
                FirstName = userSignUpDto.FirstName,
                LastName = userSignUpDto.LastName,
                DateOfBirth = userSignUpDto.DateOfBirth,
                Email = userSignUpDto.Email,
                Password = HashPassword(userSignUpDto.Password),
                ConfirmPassword = HashPassword(userSignUpDto.ConfirmPassword),
                Role = "Candidat",
                IsArchived = false,
                Username = userSignUpDto.Email,
                ValidationToken = validationToken,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserRegistrationTemps.Add(tempUser);
            await _context.SaveChangesAsync();

            // Send email verification
            await _emailService.SendEmailVerification(userSignUpDto.Email, validationToken);

            return tempUser.Id;
        }


        public async Task<User> GetUserByValidationToken(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ValidationToken == token);
        }

        public async Task SaveValidationToken(string email, string validationToken)
        {
            var emailVerification = new EmailVerification
            {
                Email = email,
                Token = validationToken,
                CreatedAt = DateTime.UtcNow
            };

            _context.EmailVerifications.Add(emailVerification);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> VerifyEmail(string token)
        {
            var tempUser = await _context.UserRegistrationTemps.FirstOrDefaultAsync(u => u.ValidationToken == token);
            if (tempUser == null)
            {
                throw new Exception("Invalid token.");
            }

            var user = new User
            {
                FirstName = tempUser.FirstName,
                LastName = tempUser.LastName,
                DateOfBirth = tempUser.DateOfBirth,
                Email = tempUser.Email,
                Password = tempUser.Password,
                ConfirmPassword = tempUser.ConfirmPassword,
                Role = tempUser.Role,
                IsArchived = tempUser.IsArchived,
                Username = tempUser.Username,
                IsEmailVerified = true
            };

            _context.Users.Add(user);
            _context.UserRegistrationTemps.Remove(tempUser);
            await _context.SaveChangesAsync();

            return true;
        }


        /*    private async Task SendEmailVerification(string email, int userId)
            {
                try
                {
                    var token = GenerateEmailVerificationToken(userId);
                    var verificationLink = $"https://votre-site.com/verify-email?token={token}";

                    // Envoyer l'e-mail de vérification à l'utilisateur
                    await _emailService.SendEmailVerification(email, verificationLink);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error sending email verification: {ex.Message}");
                }
            }*/

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string GenerateJwtSecretKey()
        {
            byte[] keyBytes = new byte[32]; // 256 bits
            try
            {
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(keyBytes);
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception en cas d'erreur lors de la génération de la clé
                throw new Exception("Error generating JWT secret key: " + ex.Message);
            }

            return Convert.ToBase64String(keyBytes);
        }


        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        private string GenerateEmailVerificationToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your-email-verification-key"); // Utilisation d'une clé pour signer le token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userId", userId.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        private int? DecodeEmailVerificationToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your-email-verification-key");

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value);
                return userId;
            }
            catch
            {
                // Token validation failed
                return null;
            }
        }

        public string AuthenticateUser(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);

            if (user == null || user.Password != HashPassword(password))
            {
                return null; // Retourne null si l'utilisateur n'est pas trouvé ou si le mot de passe est incorrect
            }

            // Générer et retourner le token JWT en appelant la méthode GenerateJwtSecretKey
            string jwtToken = GenerateJwtSecretKey();

            return jwtToken;
        }
        public User GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }
      /*  public async Task<string> GetUserEmailByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user?.Email; // Assurez-vous que le modèle User a une propriété Email
        }*/
    }
}
