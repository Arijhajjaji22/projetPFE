using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_plateforme_de_recurtement.Services
{
    public class AvailabilityService
    {
        private readonly AvailabilityRepository _availabilityRepository;
        private readonly ApplicationDbContext _context;

        public AvailabilityService(AvailabilityRepository availabilityRepository, ApplicationDbContext context)
        {
            _availabilityRepository = availabilityRepository;
            _context = context;
        }

        public async Task<IEnumerable<Availability>> GetAllAvailabilitiesAsync()
        {
            return await _availabilityRepository.GetAvailabilitiesAsync();
        }

        public async Task<Availability> GetAvailabilityByIdAsync(int id)
        {
            return await _availabilityRepository.GetAvailabilityByIdAsync(id);
        }

        public async Task AddOrUpdateAvailabilityAsync(Availability availabilityData)
        {
            if (availabilityData.Id == 0)
            {
                await _availabilityRepository.AddAvailabilityAsync(availabilityData);
            }
            else
            {
                await _availabilityRepository.UpdateAvailabilityAsync(availabilityData);
            }
        }

        public async Task DeleteAvailabilityAsync(int id)
        {
            await _availabilityRepository.DeleteAvailabilityAsync(id);
        }

        public async Task ConfirmDateAsync(int availabilityId, int userId)
        {
            await _availabilityRepository.ConfirmDateAsync(availabilityId, userId);
        }
        public async Task<bool> UserIdExistsAsync(int userId)
        {
            return await _context.availabilityConfirmations.AnyAsync(a => a.UserId == userId);
        }

    }
}
