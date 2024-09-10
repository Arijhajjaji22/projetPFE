using System.Collections.Generic;
using System.Linq;
using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Services;
using Microsoft.AspNetCore.Mvc;

namespace App_plateforme_de_recurtement.Repositories
{
    public class StageOfferRepository
    {
        private readonly ApplicationDbContext _context;

        public StageOfferRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public StageOffer GetOfferById(int id)
        {
            return _context.StageOffres.FirstOrDefault(o => o.Id == id);
        }
        public IEnumerable<StageOffer> GetOffers()
        {
            return _context.StageOffres.ToList();
        }

        public void AddOffer(StageOffer offer)
        {
            _context.StageOffres.Add(offer);
            _context.SaveChanges();
        }

        public void UpdateOffer(StageOffer offer)
        {
            _context.StageOffres.Update(offer);
            _context.SaveChanges();
        }
        public IEnumerable<StageOffer> GetValidatedOffers()
        {
            return _context.StageOffres.Where(o => o.Valide == true).ToList();
        }


        public void DeleteOffer(int id)
        {
            var offer = _context.StageOffres.Find(id);
            if (offer != null)
            {
                offer.IsDeleted = true;
                _context.SaveChanges();
            }
        }
        public IEnumerable<StageOffer> GetNonDeletedOffers()
        {
            return _context.StageOffres.Where(o => !o.IsDeleted).ToList();
        }
        public async Task DeleteAsync(StageOffer stageOffer)
        {
            _context.StageOffres.Remove(stageOffer);
            await _context.SaveChangesAsync();
        }
        public async Task<StageOffer> GetByIdAsync(int id)
        {
            return await _context.StageOffres.FindAsync(id);
        }



    }
}
