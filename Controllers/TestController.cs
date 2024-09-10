using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Data.Repositories;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using App_plateforme_de_recurtement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App_plateforme_de_recurtement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TestService _testService;
        private readonly ApplicationDbContext _context;
        private readonly TestRepository _userRepository;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FormController> _logger;


        public TestController(TestService testService, ApplicationDbContext context,TestRepository userRepository, EmailService emailService, IConfiguration configuration, ILogger<FormController> logger)
        {
            _testService = testService;
            _context = context;
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }
        [HttpPost("submit_test")]
        public async Task<IActionResult> SubmitTest([FromBody] Test test)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTest = await _testService.CreateTestAsync(test);
            return Ok(createdTest);
        }

    



        [HttpGet("{subject}")]
        public IActionResult GetAllTests(string subject)
        {
            var tests = _testService.GetTestsBySubject(subject);
            return Ok(tests);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTest(int id, [FromBody] Test updatedTest)
        {
            var existingTest = await _testService.GetTestByIdAsync(id);
            if (existingTest == null)
            {
                return NotFound();
            }

            existingTest.Title = updatedTest.Title;
            existingTest.Description = updatedTest.Description;
            existingTest.Duration = updatedTest.Duration;
            existingTest.Questions = updatedTest.Questions;

            await _testService.UpdateTestAsync(existingTest);

            return NoContent();
        }
        [HttpPost("add_question")]
        public async Task<IActionResult> AddQuestion([FromBody] Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ajoutez ici la logique pour enregistrer la question dans la base de données

            var addedQuestion = await _testService.AddQuestionAsync(question);

            return Ok(addedQuestion);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var existingTest = await _testService.GetTestByIdAsync(id);
            if (existingTest == null)
            {
                return NotFound();
            }

            await _testService.DeleteTestAsync(existingTest);

            return NoContent();
        }
        [HttpGet("{id}/questions")]
        public async Task<IActionResult> GetQuestionsByTestId(int id)
        {
            var questions = await _testService.GetQuestionsByTestIdAsync(id);
            if (questions == null || questions.Count == 0)
            {
                return NotFound();
            }

            return Ok(questions);
        }

        [HttpPost("add_answer")]
        public async Task<IActionResult> AddAnswer([FromBody] Answer answer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addedAnswer = await _testService.AddAnswerAsync(answer);

            return Ok(addedAnswer);
        }
        [HttpPost("submit_answers")]
        public async Task<IActionResult> SubmitTestcandidat([FromBody] Answercandidat[] answers)
        {
            if (answers == null || answers.Length == 0)
            {
                ModelState.AddModelError("", "No answers submitted.");
                return BadRequest(ModelState);
            }

            foreach (var answer in answers)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }

            var submittedAnswers = await _testService.SubmitAnswersAsync(answers);
            return Ok(submittedAnswers);
        }

        [HttpGet("{id}/submitted_answers")]
        public async Task<IActionResult> GetSubmittedAnswers(int id)
        {
            var answers = await _testService.GetSubmittedAnswersAsync(id);
            if (answers == null || answers.Count == 0)
            {
                return NotFound();
            }

            return Ok(answers);
        }
        [HttpGet("question/{questionId}/answercandidats")]
        public async Task<IActionResult> GetAnswercandidatsByQuestionId(int questionId)
        {
            var answercandidats = await _testService.GetAnswercandidatsByQuestionIdAsync(questionId);
            if (answercandidats == null || answercandidats.Count == 0)
            {
                return NotFound();
            }

            return Ok(answercandidats);
        }
        [HttpGet("{id}/duration")]
        public async Task<IActionResult> GetTestDurationById(int id)
        {
            var test = await _testService.GetTestByIdAsync(id);
            if (test == null)
            {
                return NotFound();
            }

            return Ok(test.Duration);
        }

        [HttpGet("calculate")]
        public async Task<IActionResult> CalculateScore()
        {
            var score = await _testService.CompareAndCalculateScoreAsync();
            return Ok(score);
        }
        [HttpGet("{id}/answers")]
        public async Task<IActionResult> GetAnswersByTestId(int id)
        {
            try
            {
                var answers = await _testService.GetAnswercandidatsByTestIdAsync(id);
                if (answers == null || answers.Count == 0)
                {
                    return NotFound("Aucune réponse trouvée pour ce test.");
                }

                return Ok(answers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des réponses : {ex.Message}");
            }
        }
        [HttpPost("savePdf")]
        public async Task<IActionResult> SavePdf([FromBody] SavePdfRequest request)
        {
            try
            {
                // Convertir la chaîne Base64 du PDF en tableau de bytes
                byte[] pdfBytes = Convert.FromBase64String(request.PdfData);

                // Enregistrer pdfBytes dans la base de données, associé à l'utilisateur connecté ou à un identifiant unique du test
                var pdfDocument = new PdfDocument
                {
                    UserId = GetCurrentUserId(), // À définir selon votre logique pour récupérer l'ID de l'utilisateur
                    PdfData = pdfBytes
                };

                _context.PdfDocuments.Add(pdfDocument);
                await _context.SaveChangesAsync();

                return Ok("PDF enregistré avec succès.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'enregistrement du PDF : {ex.Message}");
            }
        }

        public string GetCurrentUserId()
        {
            // Implémentez votre logique pour récupérer l'ID de l'utilisateur actuellement connecté
            // Peut-être à partir de l'authentification JWT ou de la session
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        // Exemple d'action de contrôleur pour récupérer le PDF de rapport de test par ID de formulaire
        [HttpGet("/api/PdfDocument/{formId}")]
        public IActionResult GetRapportTest(int formId)
        {
            var pdfDocument = _context.PdfDocuments.FirstOrDefault(p => p.Id == formId); // Adapter selon votre logique
            if (pdfDocument != null)
            {
                return File(pdfDocument.PdfData, "application/pdf");
            }
            else
            {
                return NotFound();
            }
        }
        /* [HttpPost("associateForm")]
         public async Task<IActionResult> AssociateForm(int userId, int testId)
         {
             var form = _context.Forms.FirstOrDefault(f => f.UserId == userId);

             if (form == null)
             {
                 return NotFound(new { message = "Form not found for the given user ID." });
             }

             form.TestId = testId;
             _context.Forms.Update(form);
             await _context.SaveChangesAsync();

             return Ok(new { message = "Test ID associated with form successfully." });
         }*/

        [HttpPost("sendScoreEmail")]
        public async Task<IActionResult> SendScoreEmail([FromBody] EmailRequest request)
        {
            try
            {
                // Appeler la méthode SendEmail avec les paramètres appropriés
                await _emailService.SendEmail(request.Email, request.Subject, request.Body);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail([FromBody] int userId)
        {
            _logger.LogInformation("SendTestEmail called with UserId: {UserId}", userId);

            if (userId <= 0)
            {
                _logger.LogError("Invalid UserId: {UserId}", userId);
                return BadRequest("Invalid UserId.");
            }

            try
            {
                // Récupérer l'email de l'utilisateur
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogError("User not found for UserId: {UserId}", userId);
                    return NotFound("User not found.");
                }

                var email = user.Email;
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogError("Email not found for UserId: {UserId}", userId);
                    return BadRequest("Email not found.");
                }

                // Envoyer l'email
                var subject = "Votre rapport de test";
                var message = "Vous pouvez consulter votre rapport de test en pièce jointe.";

                await _emailService.SendEmail(email, subject, message);

                _logger.LogInformation("Email sent successfully to UserId: {UserId}", userId);

                return Ok("Email sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the email.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("count_questions")]
        public async Task<IActionResult> GetQuestionCount()
        {
            try
            {
                // Calculer le nombre de questions dans la table Question
                var questionCount = await _context.Question.CountAsync();

                return Ok(new { TotalQuestions = questionCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération du nombre de questions : {ex.Message}");
            }
        }


    }
}

