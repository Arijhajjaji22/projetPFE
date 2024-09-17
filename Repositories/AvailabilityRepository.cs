using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Models;
using Microsoft.EntityFrameworkCore;

namespace App_plateforme_de_recurtement.Repositories
{
    public class AvailabilityRepository
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Availability>> GetAvailabilitiesAsync()
        {
            return await _context.Availabilities.ToListAsync();
        }

        public async Task<Availability> GetAvailabilityByIdAsync(int id)
        {
            return await _context.Availabilities.FindAsync(id);
        }

        public async Task AddAvailabilityAsync(Availability availability)
        {
            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAvailabilityAsync(Availability availability)
        {
            _context.Entry(availability).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAvailabilityAsync(int id)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability != null)
            {
                _context.Availabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }
        }
        public async Task ConfirmDateAsync(int availabilityId, int userId)
        {
            var confirmation = await _context.availabilityConfirmations
                .FirstOrDefaultAsync(ac => ac.AvailabilityId == availabilityId && ac.UserId == userId);

            if (confirmation == null)
            {
                confirmation = new AvailabilityConfirmation
                {
                    AvailabilityId = availabilityId,
                    UserId = userId,
                    IsConfirmed = true
                };
                _context.availabilityConfirmations.Add(confirmation);
            }
            else
            {
                confirmation.IsConfirmed = true;
                _context.Entry(confirmation).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }
    }
}
