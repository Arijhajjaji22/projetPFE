using System.Collections.Generic;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;

namespace App_plateforme_de_recurtement.Services
{
    public class AdminStageOfferService
    {
        private readonly AdminStageOfferRepository _adminStageOfferRepository;
        private readonly MappingService _mappingService;

        public AdminStageOfferService(AdminStageOfferRepository adminStageOfferRepository, MappingService mappingService)
        {
            _adminStageOfferRepository = adminStageOfferRepository;
            _mappingService = mappingService;
        }

        public void MoveOfferToAdminStageOffers(StageOffer offer, string validatedByUserId)
        {
            var adminStageOffer = new AdminStageOffer
            {
                Titre = offer.Titre,
                Description = offer.Description,
                DomaineActivite = offer.DomaineActivite,
                CompetencesRequises = string.Join(", ", offer.CompetencesRequises),
                DateDebut = offer.DateDebut,
                DateFin = offer.DateFin,
                NiveauEtudesRequis = offer.NiveauEtudesRequis,
                TypedeStage = offer.TypedeStage,
                //ValidatedByUserId = validatedByUserId // Assigner l'ID de l'utilisateur validant
            };

            _adminStageOfferRepository.AddOffer(adminStageOffer);
        }

        // Ajoutez une méthode AddOffer ici si nécessaire
        public void AddOffer(AdminStageOffer offer)
        {
            _adminStageOfferRepository.AddOffer(offer);
        }
        public void DeleteAdminOffer(AdminStageOffer offer)
        {
            _adminStageOfferRepository.DeleteOffer(offer);
        }
        public AdminStageOffer GetAdminOfferById(int id)
        {
            return _adminStageOfferRepository.GetAdminOfferById(id);
        }
        public void DeleteAdminOfferById(int id)
        {
            var offer = _adminStageOfferRepository.GetAdminOfferById(id);
            if (offer != null)
            {
                _adminStageOfferRepository.DeleteOffer(offer);
            }
            // Vous pouvez gérer ici le cas où l'offre n'existe pas
        }
        public void UpdateOffer(AdminStageOffer offer)
        {
            _adminStageOfferRepository.UpdateOffer(offer);
        }

        public void DisapproveOffer(int id)
        {
            var offer = _adminStageOfferRepository.GetAdminOfferById(id);
            if (offer != null)
            {
                offer.Desapprouve = true; // Changer la valeur de la propriété 'Desapprouve' de false à true
                offer.Approved = false; // Mettre à jour le statut d'approbation à false
                _adminStageOfferRepository.UpdateOffer(offer);
            }
        }


        public IEnumerable<AdminStageOffer> GetNonDisapprovedOffers()
        {
            return _adminStageOfferRepository.GetNonDisapprovedOffers();
        }
        public void ApproveOffer(int id)
        {
            var offer = _adminStageOfferRepository.GetAdminOfferById(id);
            if (offer != null)
            {
                offer.Approved = true; // Changer la valeur de la propriété 'Approved' à true
                _adminStageOfferRepository.UpdateOffer(offer);
            }
        }

        public IEnumerable<AdminStageOffer> GetApprovedOffers()
        {
            return _adminStageOfferRepository.GetApprovedOffers();
        }


    }

}
