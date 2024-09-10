using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App_plateforme_de_recurtement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly AvailabilityService _availabilityService;

        public AvailabilityController(AvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        // POST: api/Availability
        [HttpPost]
        public async Task<ActionResult<Availability>> SaveAvailability([FromBody] Availability availabilityData)
        {
            if (ModelState.IsValid)
            {
                await _availabilityService.AddOrUpdateAvailabilityAsync(availabilityData);
                return Ok(availabilityData); // Retourne l'objet de disponibilité avec l'ID mis à jour
            }
            return BadRequest(ModelState);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Availability>>> GetAvailabilities()
        {
            var availabilities = await _availabilityService.GetAllAvailabilitiesAsync();
            return Ok(availabilities);
        }

        // DELETE: api/Availability/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Availability>> DeleteAvailability(int id)
        {
            var availability = await _availabilityService.GetAvailabilityByIdAsync(id);
            if (availability == null)
            {
                return NotFound();
            }

            await _availabilityService.DeleteAvailabilityAsync(id);
            return Ok(availability);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] Availability availabilityData)
        {
            if (id != availabilityData.Id)
            {
                return BadRequest();
            }

            await _availabilityService.AddOrUpdateAvailabilityAsync(availabilityData);

            // Renvoie l'événement mis à jour en tant que réponse
            return Ok(availabilityData);
        }
        [HttpPost("{id}/confirm")]
        public async Task<ActionResult> ConfirmAvailability(int id, [FromBody] int userId)
        {
            await _availabilityService.ConfirmDateAsync(id, userId);
            return NoContent();
        }
        // Nouvelle action pour vérifier l'existence du UserId
        [HttpGet("CheckUserId/{userId}")]
        public async Task<ActionResult<string>> CheckUserId(int userId)
        {
            bool exists = await _availabilityService.UserIdExistsAsync(userId);
            if (exists)
            {
                return Ok("Oui");
            }
            return Ok("Non");
        }
    }

}
