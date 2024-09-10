using System.Collections.Generic;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using Microsoft.EntityFrameworkCore;

namespace App_plateforme_de_recurtement.Services
{
    public class StageOfferService
    {
        private readonly StageOfferRepository _stageOfferRepository;
        private readonly AdminStageOfferService _adminStageOfferService;

        public StageOfferService(StageOfferRepository stageOfferRepository, AdminStageOfferService adminStageOfferService)
        {
            _stageOfferRepository = stageOfferRepository;
            _adminStageOfferService = adminStageOfferService;
        }
        public void ApproveOffer(int id, string validatedByUserId)
        {
            var offer = _stageOfferRepository.GetOfferById(id);
            if (offer != null)
            {
                offer.Approved = true;
                _stageOfferRepository.UpdateOffer(offer);

                // Déplacer l'offre vers adminStageOffers
                _adminStageOfferService.MoveOfferToAdminStageOffers(offer, validatedByUserId); // Passer l'ID de l'utilisateur validant
            }
        }

        public IEnumerable<StageOffer> GetOffers()
        {
            return _stageOfferRepository.GetOffers();
        }
        public StageOffer GetOfferById(int id)
        {
            return _stageOfferRepository.GetOfferById(id);
        }


        public void AddOffer(StageOffer offer)
        {
            _stageOfferRepository.AddOffer(offer);
        }
        public IEnumerable<StageOffer> GetValidatedOffers()
        {
            return _stageOfferRepository.GetValidatedOffers();
        }


        public void UpdateOffer(StageOffer offer)
        {
            _stageOfferRepository.UpdateOffer(offer);
        }

        public void DeleteOffer(int id)
        {
            _stageOfferRepository.DeleteOffer(id);
        }
        public void MarkOfferAsDeleted(int id)
        {
            var offer = _stageOfferRepository.GetOfferById(id);
            if (offer != null)
            {
                offer.IsDeleted = true;
                _stageOfferRepository.UpdateOffer(offer);
            }
        }
        public IEnumerable<StageOffer> GetNonDeletedOffers()
        {
            return _stageOfferRepository.GetNonDeletedOffers();
        }
        public async Task<bool> DeleteStageOfferAsync(int id)
        {
            // Logique pour supprimer l'offre de stage de la base de données
            var stageOffer = await _stageOfferRepository.GetByIdAsync(id);
            if (stageOffer == null)
                return false;

            await _stageOfferRepository.DeleteAsync(stageOffer);
            return true;
        }
        public IEnumerable<StageOffer> GetApprovedOffers()
        {
            return _stageOfferRepository.GetOffers().Where(o => o.Approved);
        }


    }
}
