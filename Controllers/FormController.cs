using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.DTOs;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using App_plateforme_de_recurtement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EmailDto = App_plateforme_de_recurtement.DTOs.EmailDto;
using EmailRequest = App_plateforme_de_recurtement.DTOs.EmailRequest;

namespace App_plateforme_de_recurtement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly FormService _formService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormController> _logger;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly FormRepository _formRepository;
        private readonly UserService _userService;
        private readonly AvailabilityConfirmationService _availabilityConfirmationService;
        public FormController(FormService formService, ApplicationDbContext context, ILogger<FormController> logger, EmailService emailService, IConfiguration configuration, FormRepository formRepository, UserService userService, AvailabilityConfirmationService availabilityConfirmationService)
        {
            _formService = formService;
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
            _formRepository = formRepository;
            _userService = userService;
            _availabilityConfirmationService = availabilityConfirmationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Form>>> GetAllForms()
        {
            var forms = await _formService.GetAllFormsAsync();
            return Ok(forms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Form>> GetFormById(int id)
        {
            var form = await _formService.GetFormByIdAsync(id);
            if (form == null)
            {
                return NotFound();
            }
            return Ok(form);
        }

        [HttpPost]
        public async Task<ActionResult<Form>> AddForm([FromForm] FormDto formDto)
        {
            var documentValidator = new DocumentValidator();

            // Vérification du CV
            if (formDto.CV != null)
            {
                var tempCvPath = Path.GetTempFileName();
                using (var stream = new FileStream(tempCvPath, FileMode.Create))
                {
                    await formDto.CV.CopyToAsync(stream);
                }

                if (!documentValidator.IsPdfCv(tempCvPath))
                {
                    return BadRequest("Le fichier CV soumis n'est pas un CV valide.");
                }
            }

            // Vérification de la lettre de motivation
            if (formDto.LettreMotivation != null)
            {
                var tempLmPath = Path.GetTempFileName();
                using (var stream = new FileStream(tempLmPath, FileMode.Create))
                {
                    await formDto.LettreMotivation.CopyToAsync(stream);
                }

                if (!documentValidator.IsPdfCoverLetter(tempLmPath))
                {
                    return BadRequest("Le fichier de lettre de motivation soumis n'est pas valide.");
                }
            }

            var form = new Form
            {
                Prenom = formDto.Prenom,
                Nom = formDto.Nom,
                Email = formDto.Email,
                Telephone = formDto.Telephone,
                Faculte = formDto.Faculte,
                NiveauEtudes = formDto.NiveauEtudes,
                CVData = formDto.CV != null ? await ConvertFileToBytes(formDto.CV) : null,
                LettreMotivationData = formDto.LettreMotivation != null ? await ConvertFileToBytes(formDto.LettreMotivation) : null,
                AdminStageOfferId = formDto.AdminStageOfferId,
            };

            var newForm = await _formService.AddFormAsync(form);

            var response = new
            {
                Id = newForm.Id,
                // Autres champs si nécessaire
            };

            return CreatedAtAction(nameof(GetFormById), new { id = newForm.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(int id, [FromForm] FormDto formDto)
        {
            var form = await _formService.GetFormByIdAsync(id);
            if (form == null)
            {
                return NotFound();
            }

            form.Prenom = formDto.Prenom;
            form.Nom = formDto.Nom;
            form.Email = formDto.Email;
            form.Telephone = formDto.Telephone;
            form.Faculte = formDto.Faculte;
            form.NiveauEtudes = formDto.NiveauEtudes;
            form.CVData = formDto.CV != null ? await ConvertFileToBytes(formDto.CV) : form.CVData;
            form.LettreMotivationData = formDto.LettreMotivation != null ? await ConvertFileToBytes(formDto.LettreMotivation) : form.LettreMotivationData;

            await _formService.UpdateFormAsync(form);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            await _formService.DeleteFormAsync(id);
            return NoContent();
        }

        private async Task<byte[]> ConvertFileToBytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [HttpGet("stageoffer/{stageOfferId}")]
        public async Task<ActionResult<IEnumerable<Form>>> GetFormsByStageOfferId(int stageOfferId)
        {
            var forms = await _formService.GetFormsByStageOfferIdAsync(stageOfferId);
            return Ok(forms);
        }
        [HttpGet("count/{stageOfferId}")]
        public async Task<ActionResult<int>> CountFormsByStageOfferId(int stageOfferId)
        {
            var count = await _formService.CountFormsByStageOfferIdAsync(stageOfferId);
            return Ok(count);
        }
        [HttpPost("saveTestReport")]
        public async Task<IActionResult> SaveTestReport([FromBody] TestReportDto testReport)
        {
            _logger.LogInformation("SaveTestReport called with UserId: {UserId}, TestId: {TestId}", testReport.UserId, testReport.TestId);

            if (testReport == null || testReport.UserId <= 0 || testReport.TestId <= 0 || string.IsNullOrEmpty(testReport.Report))
            {
                _logger.LogError("Invalid data received: {TestReport}", testReport);
                return BadRequest("Invalid data.");
            }

            try
            {
                // Convertir le rapport PDF en tableau de bytes
                var reportData = Convert.FromBase64String(testReport.Report);

                // Créer une instance de Form pour stocker les informations
                var form = new Form
                {
                    UserId = testReport.UserId,
                    //TestId = testReport.TestId,
                    PdfReportData = reportData,
                    Email = testReport.Email, // Stocker l'email
                    CreatedAt = DateTime.UtcNow // Ajouter une date de création
                };

                // Ajouter le Form à la base de données
                _context.Forms.Add(form);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Report saved successfully for UserId: {UserId}, TestId: {TestId}", testReport.UserId, testReport.TestId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the test report.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("sendEmailIfScoreIsHigh")]
        public async Task<IActionResult> SendEmailIfScoreIsHigh([FromBody] Models.EmailDto emailDto)
        {
            if (emailDto == null || string.IsNullOrEmpty(emailDto.Email))
            {
                return BadRequest("Invalid email data.");
            }

            try
            {
                // Rechercher le formulaire correspondant à l'email donné
                var form = await _context.Forms.FirstOrDefaultAsync(f => f.Email == emailDto.Email);
                if (form == null)
                {
                    return NotFound("No form found with the provided email.");
                }

                // Construire l'URL du lien pour choisir une date pour un entretien
                var interviewUrl = Url.Action("Calendriercandidat", "Home", null, Request.Scheme);

                // Construire le corps de l'email avec le lien et le prénom
                var emailBody = $"Bonjour {form.Prenom},\n\nMerci d'avoir complété le test. Veuillez cliquer sur le lien suivant pour sélectionner une date pour votre entretien : {interviewUrl}  \n\nCordialement,\nL'équipe de recrutement";

                // Utiliser l'EmailService pour envoyer un email
                await _emailService.SendEmail(
                    recipientEmail: emailDto.Email,
                    subject: "Invitation à un entretien",
                    body: emailBody
                );

                return Ok("Email envoyé avec succès.");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while sending the email.");
                return StatusCode(500, $"An error occurred while sending the email: {ex.Message}");
            }
        }

        [HttpGet("report/{userId}")]
        public async Task<IActionResult> GetTestReport(int userId)
        {
            var form = await _context.Forms
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PdfReportData != null);

            if (form == null)
            {
                return NotFound("Rapport de test non trouvé pour l'utilisateur spécifié.");
            }

            return File(form.PdfReportData, "application/pdf", "rapport_test.pdf");
        }
        // Action pour accepter un candidat
        [HttpPost("accept/{id}")]
        public async Task<IActionResult> AcceptCandidate(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound(new { message = "Formulaire non trouvé." });
            }

            // Mettre à jour l'état du formulaire à "Accepté"
            form.Status = "Accepté"; // Assurez-vous que vous avez un champ Status dans votre modèle Form
            await _context.SaveChangesAsync();

            // Optionnel: Envoyer un email pour notifier le candidat
            // await _emailService.SendEmail(form.Email, "Invitation à un entretien", "Félicitations, vous avez été accepté!");

            // Retourner un résultat JSON avec un message et l'ID du formulaire
            return Ok(new { message = "Candidat accepté.", id = id });
        }

        // Action pour envoyer un email de rejet
        [HttpPost("sendRejectionEmail/{id}")]
        public async Task<IActionResult> SendRejectionEmail(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound("Formulaire non trouvé.");
            }

            var emailSubject = "Réponse à votre candidature";
            var emailBody = "Désolé, vous n'avez pas été retenu. Nous vous souhaitons bonne chance pour vos futures candidatures.";

            // Envoyer l'email
            await _emailService.SendEmail(form.Email, emailSubject, emailBody);

            return Ok("Email de rejet envoyé.");
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectCandidate(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound("Formulaire non trouvé.");
            }

            // Mettre à jour l'état du formulaire à "Rejeté"
            form.Status = "Rejeté"; // Assurez-vous que vous avez un champ Status dans votre modèle Form
            await _context.SaveChangesAsync();

            // Préparer le contenu de l'email
            var subject = "Réponse à votre candidature";
            var body = $"Bonjour {form.Prenom},\n\n" +
                       "Désolé, vous n'avez pas été retenu pour ce poste. Nous vous souhaitons bonne chance pour vos futures candidatures.\n\n" +
                       "Cordialement,\nL'équipe de recrutement";

            // Envoyer l'email
            await _emailService.SendEmail(form.Email, subject, body);

            return Ok("Candidat rejeté.");
        }



        [HttpPost("sendEmail/{id}")]
        public async Task<IActionResult> SendEmail(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound();
            }

            var emailDto = new App_plateforme_de_recurtement.DTOs.EmailDto
            {
                To = form.Email,
                Subject = "Votre candidature a été acceptée",
                Body = $"Bonjour {form.Prenom},\n\nVotre candidature pour le poste a été acceptée.\n\nCordialement,\nL'équipe de recrutement"
            };

            try
            {
                await _emailService.SendEmailAsync(emailDto);
                return Ok(new { message = "Email envoyé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur lors de l'envoi de l'email", error = ex.Message });
            }
        }
        [HttpPost("updateInterviewStatus/{id}")]
        public async Task<IActionResult> UpdateInterviewStatus(int id, [FromBody] bool interviewPassed)
        {
            await _formService.UpdateInterviewPassedAsync(id, interviewPassed);
            return NoContent();
        }


        [HttpPost("{id}/rating")]
        public async Task<IActionResult> UpdateStarRating(int id, [FromBody] int starRating)
        {
            if (starRating < 1 || starRating > 5)
            {
                return BadRequest("La note doit être comprise entre 1 et 5 étoiles.");
            }

            await _formService.UpdateStarRatingAsync(id, starRating);
            return Ok();
        }
        // Endpoint pour récupérer la note d'un formulaire par son ID
        [HttpGet("{formId}/rating")]
        public async Task<ActionResult<int>> GetRating(int formId)
        {
            var form = await _context.Forms.FindAsync(formId);
            if (form == null)
            {
                return NotFound();
            }
            return form.StarRating; // Assurez-vous que la propriété Rating est bien définie dans votre modèle
        }
        [HttpPost("checkEmailExists")]
        public async Task<IActionResult> CheckEmailExists([FromBody] EmailCheckDto emailCheckDto)
        {
            if (emailCheckDto == null || string.IsNullOrEmpty(emailCheckDto.Email))
            {
                return BadRequest("Email is required.");
            }

            var emailExists = await _context.Forms.AnyAsync(f => f.Email == emailCheckDto.Email);

            if (emailExists)
            {
                return BadRequest("Cet email existe déjà, veuillez saisir un autre email.");
            }

            return Ok("L'email est disponible.");
        }
        [HttpPost("RescheduleTest")]
        public async Task<IActionResult> RescheduleTest([FromBody] RescheduleRequesttest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        _logger.LogError("Erreur de validation : {Error}", error.ErrorMessage);
                    }
                    return BadRequest("Requête invalide.");
                }

                if (request.UserId <= 0 || string.IsNullOrEmpty(request.Subject))
                {
                    _logger.LogWarning("UserId ou Subject manquant. UserId: {UserId}, Subject: {Subject}", request.UserId, request.Subject);
                    return BadRequest("UserId ou Subject manquant.");
                }

                // Enregistrement dans la table RescheduleRequesttest
                _context.rescheduleRequesttests.Add(request);
                await _context.SaveChangesAsync();

                // Récupérer le test basé sur le Subject
                var test = await _context.Tests
                    .FirstOrDefaultAsync(t => t.Subject == request.Subject);

                if (test == null)
                {
                    _logger.LogWarning("Test non trouvé pour Subject: {Subject}", request.Subject);
                    return NotFound("Test non trouvé.");
                }

                // Récupérer l'email de l'utilisateur
                var form = await _context.Forms.FirstOrDefaultAsync(f => f.Id == request.UserId);
                if (form != null && !string.IsNullOrEmpty(form.Email))
                {
                    // Construire le lien du test en utilisant Subject
                    var testLink = GenerateTestLink(request.Subject); // Utilisation de Subject

                    // Construire le corps de l'email de confirmation avec le lien du test
                    var confirmationEmailBody = $"Bonjour {form.Prenom},\n\nVotre test a été reprogrammé au {request.NewDateTime:yyyy-MM-dd HH:mm:ss}.\nCordialement,\nL'équipe de recrutement";

                    var confirmationEmailDto = new EmailDto
                    {
                        To = form.Email,
                        Subject = "Rappel : Votre test a été reprogrammé",
                        Body = confirmationEmailBody
                    };

                    await _emailService.SendEmailAsync(confirmationEmailDto);

                    // Planifier l'envoi d'un rappel de l'email avec le lien de test
                    ScheduleTestEmail(form.Email, request.Subject, request.NewDateTime);
                }

                return Ok("Test reprogrammé avec succès.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la replanification du test. Détails : {Message}", ex.Message);
                return StatusCode(500, "Une erreur interne est survenue.");
            }
        }


        private void ScheduleTestEmail(string email, string subject, DateTime? newDateTime)
        {
            if (newDateTime == null)
            {
                _logger.LogWarning("La date du test est invalide.");
                return;
            }

            var dateTime = newDateTime.Value;

            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            }

            var delay = dateTime - DateTime.Now;

            if (delay.TotalMilliseconds > 0)
            {
                var timer = new Timer(async _ =>
                {
                    var testLink = GenerateTestLink(subject); // Utilisation de Subject

                    var testEmailBody = $"Bonjour,\n\nVotre test est maintenant disponible. Vous pouvez y accéder via le lien suivant :\n{testLink}\n\nCordialement,\nL'équipe de recrutement";

                    var testEmailDto = new EmailDto
                    {
                        To = email,
                        Subject = "Votre test est prêt",
                        Body = testEmailBody
                    };

                    await _emailService.SendEmailAsync(testEmailDto);
                }, null, delay, Timeout.InfiniteTimeSpan);
            }
            else
            {
                var testLink = GenerateTestLink(subject); // Utilisation de Subject
                var testEmailBody = $"Bonjour,\n\nVotre test est maintenant disponible. Vous pouvez y accéder via le lien suivant :\n{testLink}\n\nCordialement,\nL'équipe de recrutement";

                var testEmailDto = new EmailDto
                {
                    To = email,
                    Subject = "Votre test est prêt",
                    Body = testEmailBody
                };

                _emailService.SendEmailAsync(testEmailDto).GetAwaiter().GetResult();
            }
        }

        private string GenerateTestLink(string subjectId)
        {
            var url = Url.Action("testdetailsmail", "Home", new { subjectId = subjectId }, Request.Scheme);
            return url;
        }
        [HttpPost]
        [Route("SendConfirmationEmail")]
        public async Task<IActionResult> SendConfirmationEmail([FromBody] EmailRequest emailRequest)
        {
            if (emailRequest == null || string.IsNullOrEmpty(emailRequest.Email))
            {
                return BadRequest("Invalid email request");
            }

            // Corps de l'email
            var emailBody = "Bonjour,\n\n" +
                             "Merci d'avoir sélectionné une date. Nous avons bien reçu votre choix.\n\n" +
                             "Si vous avez des questions ou des préoccupations, n'hésitez pas à nous contacter.\n\n" +
                             "Cordialement,\n";

            // Utilisez votre EmailService pour envoyer l'email
            await _emailService.SendEmail(
                emailRequest.Email,
                "Confirmation de la date",
                emailBody
            );

            return Ok("Email envoyé avec succès");
        }

    }
}
