using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Models;
using Microsoft.EntityFrameworkCore;

namespace App_plateforme_de_recurtement.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Méthode asynchrone pour ajouter un utilisateur
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        /* public void ArchiveUser(int id)
         {
             var user = _context.Users.FirstOrDefault(u => u.Id == id);
             if (user != null)
             {
                 _context.Users.Remove(user);
                 _context.SaveChanges();
             }
         }*/
        public void DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        public bool IsEmailAlreadyExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
        public async Task<User> GetUserByValidationToken(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ValidationToken == token);
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
   
    }
}

 
 