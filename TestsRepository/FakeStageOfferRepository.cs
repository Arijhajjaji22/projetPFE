using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace App_plateforme_de_recrutement.Tests.Repository
{
    public class FakeStageOfferRepository : StageOfferRepository
    {
        private readonly List<StageOffer> _offers = new List<StageOffer>();

        public FakeStageOfferRepository() : base(null) // Passez null ou un contexte factice si nécessaire
        {
            // Initialisation des données fictives si nécessaire
        }


        // Marquer les méthodes comme virtuelles pour pouvoir les substituer
        public override StageOffer GetOfferById(int id)
        {
            return _offers.FirstOrDefault(o => o.Id == id);
        }

        public override IEnumerable<StageOffer> GetOffers()
        {
            return _offers;
        }

        public override void UpdateOffer(StageOffer offer)
        {
            var existingOffer = _offers.FirstOrDefault(o => o.Id == offer.Id);
            if (existingOffer != null)
            {
                existingOffer.Approved = offer.Approved;
            }
        }

        // Méthode pour ajouter des offres dans le dépôt fictif
        public void AddOffer(StageOffer offer)
        {
            _offers.Add(offer);
        }
    }
}
