using App_plateforme_de_recurtement.Data;
using Microsoft.EntityFrameworkCore;

namespace App_plateforme_de_recurtement.Services
{
    public class AvailabilityConfirmationService
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityConfirmationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetUserIdByAvailabilityIdAsync(int availabilityId)
        {
            var confirmation = await _context.availabilityConfirmations
                .Where(ac => ac.AvailabilityId == availabilityId && ac.IsConfirmed)
                .Select(ac => ac.UserId)
                .FirstOrDefaultAsync();

            return confirmation;
        }
        public async Task<string> GetUserEmailByIdAsync(int userId)
        {
            var user = await _context.Forms
                .Where(f => f.Id == userId)
                .Select(f => f.Email)
                .FirstOrDefaultAsync();

            return user;
        }


    }

}
