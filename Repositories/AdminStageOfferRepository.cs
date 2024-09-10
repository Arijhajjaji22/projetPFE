using System.Collections.Generic;
using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Models;

namespace App_plateforme_de_recurtement.Repositories
{
    public class AdminStageOfferRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminStageOfferRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public AdminStageOffer GetAdminOfferById(int id)
        {
            return _context.adminStageOffers.FirstOrDefault(a => a.Id == id);
        }
        public void AddOffer(AdminStageOffer offer)
        {
            _context.adminStageOffers.Add(offer);
            _context.SaveChanges();
        }
        public void UpdateDesapprovalStatus(int id, bool desapproved)
        {
            var offer = _context.adminStageOffers.FirstOrDefault(a => a.Id == id);
            if (offer != null)
            {
                offer.Desapprouve = desapproved; // Mettre à jour la valeur de 'Desapprouve'
                _context.SaveChanges();
            }
        }

        public void DeleteOffer(AdminStageOffer offer)
        {
            _context.adminStageOffers.Remove(offer);
            _context.SaveChanges();
        }
        public void UpdateOffer(AdminStageOffer offer)
        {
            _context.adminStageOffers.Update(offer);
            _context.SaveChanges();
        }
        public IEnumerable<AdminStageOffer> GetNonDisapprovedOffers()
        {
            return _context.adminStageOffers.Where(o => !o.Desapprouve).ToList();
        }
        public IEnumerable<AdminStageOffer> GetApprovedOffers()
        {
            return _context.adminStageOffers.Where(o => o.Approved).ToList();
        }


    }
}
