using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Services;

namespace App_plateforme_de_recurtement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StageOfferController : ControllerBase
    {
        private readonly StageOfferService _stageOfferService;
        private readonly AdminStageOfferService _adminStageOfferService;
        private readonly NotificationService _notificationService;

        public StageOfferController(StageOfferService stageOfferService, AdminStageOfferService adminStageOfferService, NotificationService notificationService)
        {
            _stageOfferService = stageOfferService;
            _adminStageOfferService = adminStageOfferService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<StageOffer>> Get()
        {
            var offers = _stageOfferService.GetOffers();
            return Ok(offers);
        }

        [HttpPost]
        public ActionResult<StageOffer> Create(StageOffer offer)
        {

            _stageOfferService.AddOffer(offer);
            return CreatedAtAction(nameof(Get), new { id = offer.Id }, offer);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, StageOffer offer)
        {
            if (id != offer.Id)
            {
                return BadRequest();
            }

            _stageOfferService.UpdateOffer(offer);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var offer = _stageOfferService.GetOfferById(id);
            if (offer == null)
            {
                return NotFound();
            }

            // Marquer l'offre comme supprimée dans la base de données
            _stageOfferService.MarkOfferAsDeleted(id); // Passer l'ID de l'offre à supprimer

            return NoContent();
        }

        [HttpGet("notdisapproved")]
        public ActionResult<IEnumerable<AdminStageOffer>> GetNonDisapprovedOffers()
        {
            var nonDisapprovedOffers = _adminStageOfferService.GetNonDisapprovedOffers();
            return Ok(nonDisapprovedOffers);
        }
        [HttpGet("validated")]
        public ActionResult<IEnumerable<StageOffer>> GetValidatedSubjects()
        {
            var validatedSubjects = _stageOfferService.GetValidatedOffers();
            return Ok(validatedSubjects);
        }

        [HttpPost("validate/{id}")]
        public IActionResult Validate(int id)
        {
            var offer = _stageOfferService.GetOfferById(id);
            if (offer == null)
            {
                return NotFound();
            }

            offer.Valide = true; // Marquer le sujet comme validé dans la base de données
            _stageOfferService.UpdateOffer(offer);

            // Ajouter le sujet validé à la table adminstageoffer
            var adminStageOffer = new AdminStageOffer
            {
                // Assigner les propriétés du sujet validé à adminStageOffer
                Titre = offer.Titre,
                Description = offer.Description,
                DomaineActivite = offer.DomaineActivite,
                CompetencesRequises = string.Join(", ", offer.CompetencesRequises),
                DateDebut = offer.DateDebut,
                DateFin = offer.DateFin,
                NiveauEtudesRequis = offer.NiveauEtudesRequis,
                TypedeStage = offer.TypedeStage
                // Assurez-vous d'assigner les autres propriétés si nécessaire
            };

            // Ajouter adminStageOffer à la base de données en utilisant le service AdminStageOfferService
            _adminStageOfferService.AddOffer(adminStageOffer); // Utilisation du service AdminStageOfferService
            var notification = new Notification
            {
                Message = "Nouvelle offre de stage ajoutée",
            DateCreated = DateTime.Now,
                // Autres détails de la notification si nécessaire
            };
            _notificationService.CreateNotification(notification);

            return NoContent();
        }


        [HttpGet("notdeleted")]
        public ActionResult<IEnumerable<StageOffer>> GetNonDeletedOffers()
        {
            var nonDeletedOffers = _stageOfferService.GetNonDeletedOffers();
            return Ok(nonDeletedOffers);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteStageOffer(int id)
        {
            var offerToDelete = _adminStageOfferService.GetAdminOfferById(id);
            if (offerToDelete != null)
            {
                _adminStageOfferService.DeleteAdminOffer(offerToDelete);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete("deleteValidated/{id}")]
        public IActionResult DeleteValidated(int id)
        {
            var offerToDelete = _adminStageOfferService.GetAdminOfferById(id);
            if (offerToDelete != null)
            {
                _adminStageOfferService.DeleteAdminOffer(offerToDelete);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }








        [HttpPost("approve/{id}")]
        public IActionResult Approve(int id)
        {
            var offer = _adminStageOfferService.GetAdminOfferById(id);
            if (offer == null)
            {
                return NotFound();
            }

            _adminStageOfferService.ApproveOffer(id); // Approuver l'offre en passant son ID

            return NoContent();
        }



        [HttpPost("disapprove/{id}")]
        public IActionResult Disapprove(int id)
        {
            var offer = _adminStageOfferService.GetAdminOfferById(id);
            if (offer == null)
            {
                return NotFound();
            }

            _adminStageOfferService.DisapproveOffer(id); // Désapprouver l'offre en passant son ID

            return NoContent();
        }


        [HttpGet("admin/dashboard")]
        public IActionResult GetAdminDashboardNotifications()
        {
            var notifications = _notificationService.GetAllNotifications(); // Obtenez toutes les notifications
            return Ok(notifications);
        }
        [HttpGet("admin/allnotifications")] // Endpoint pour obtenir toutes les notifications
        public IActionResult GetAllNotifications()
        {
            var notifications = _notificationService.GetAllNotifications(); // Obtenez toutes les notifications
            return Ok(notifications);
        }
        [HttpGet("approved")]
        public ActionResult<IEnumerable<AdminStageOffer>> GetApprovedSubjects()
        {
            var approvedSubjects = _adminStageOfferService.GetApprovedOffers();
            return Ok(approvedSubjects);
        }


        [HttpGet("{id}")]
        public IActionResult GetOfferById(int id)
        {
            var offer = _stageOfferService.GetOfferById(id);
            if (offer == null)
            {
                return NotFound();
            }
            return Ok(offer);
        }


    }


}
