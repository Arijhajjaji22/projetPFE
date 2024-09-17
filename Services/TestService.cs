using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Data.Repositories;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using Microsoft.EntityFrameworkCore;

namespace App_plateforme_de_recurtement.Services
{
    public class TestService
    {
        private readonly TestRepository _repository;
        private readonly ApplicationDbContext _context;
        public TestService(TestRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;

        }

        public Task<Test> CreateTestAsync(Test test)
        {
            return _repository.AddTestAsync(test);
        }
        public async Task<List<Answercandidat>> GetAnswercandidatsByTestIdAsync(int testId)
        {
            return await _repository.GetAnswercandidatsByTestIdAsync(testId);
        }

        public List<Test> GetAllTests()
        {
            return _repository.GetAllTests();
        }
        public List<Test> GetTestsBySubject(string subject)
        {
            return _repository.GetTestsBySubject(subject);
        }
        public async Task<Test> UpdateTestAsync(Test updatedTest)
        {
            var existingTest = await _repository.GetTestByIdAsync(updatedTest.Id);

            if (existingTest != null)
            {
                existingTest.Title = updatedTest.Title;
                existingTest.Description = updatedTest.Description;
                existingTest.Duration = updatedTest.Duration;

                // Pour chaque question dans le test mis à jour
                foreach (var updatedQuestion in updatedTest.Questions)
                {
                    // Trouver la question correspondante dans le test existant
                    var existingQuestion = existingTest.Questions.FirstOrDefault(q => q.Id == updatedQuestion.Id);

                    if (existingQuestion != null)
                    {
                        // Mettre à jour les valeurs de la question existante
                        existingQuestion.Query = updatedQuestion.Query;

                        // Mettre à jour le champ Type de la question
                        existingQuestion.Type = updatedQuestion.Type;

                        // Pour chaque réponse dans la question mise à jour
                        foreach (var updatedAnswer in updatedQuestion.Answers)
                        {
                            // Trouver la réponse correspondante dans la question existante
                            var existingAnswer = existingQuestion.Answers.FirstOrDefault(a => a.Id == updatedAnswer.Id);

                            if (existingAnswer != null)
                            {
                                // Mettre à jour les valeurs de la réponse existante
                                existingAnswer.Response = updatedAnswer.Response;
                                existingAnswer.IsCorrect = updatedAnswer.IsCorrect;
                            }
                            else
                            {
                                // Si la réponse n'existe pas encore, l'ajouter à la question existante
                                existingQuestion.Answers.Add(updatedAnswer);
                            }
                        }
                    }
                    else
                    {
                        // Si la question n'existe pas encore, l'ajouter au test existant
                        existingTest.Questions.Add(updatedQuestion);
                    }
                }

                await _repository.UpdateTestAsync(existingTest);
            }

            return existingTest;
        }


        public async Task<Question> AddQuestionAsync(Question question)
        {
            // Ajouter la question à la base de données
            var addedQuestion = await _repository.AddQuestionAsync(question);

            return addedQuestion;
        }



        public async Task<Answer> AddAnswerAsync(Answer answer)
        {
            // Ajouter la réponse à la base de données
            var addedAnswer = await _repository.AddAnswerAsync(answer);

            // Vérifier si l'option est correcte
            if (addedAnswer.IsCorrect)
            {
                // Si c'est le cas, désactiver toutes les autres options de la même question
                var otherAnswers = await _repository.GetAnswersByQuestionIdAsync(addedAnswer.QuestionId);
                foreach (var otherAnswer in otherAnswers)
                {
                    if (otherAnswer.Id != addedAnswer.Id)
                    {
                        otherAnswer.IsCorrect = false;
                        await _repository.UpdateAnswerAsync(otherAnswer);
                    }
                }
            }

            return addedAnswer;
        }




        public Task<List<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            return _repository.GetQuestionsByTestIdAsync(testId);
        }


        public async Task<Test> SubmitTestAsync(Test test)
        {
            // Vous pouvez traiter les réponses ici et les enregistrer dans la base de données
            // Par exemple, vous pouvez ajouter un nouveau champ à votre modèle Test pour stocker les réponses

            var submittedTest = await _repository.AddTestAsync(test);
            return submittedTest;
        }

        public async Task DeleteTestAsync(Test test)
        {
            await _repository.DeleteTestAsync(test);
        }
        public Task<Test> GetTestByIdAsync(int id)
        {
            return _repository.GetTestByIdAsync(id);
        }

        public async Task<List<Answercandidat>> SubmitAnswersAsync(Answercandidat[] candidateAnswers)
        {
            var submittedAnswers = new List<Answercandidat>();

            // Récupérer les réponses du manager depuis la base de données
            var managerAnswers = await _repository.GetManagerAnswersAsync();

            foreach (var candidateAnswer in candidateAnswers)
            {
                // Trouver la réponse du manager correspondant à la réponse du candidat
                var managerAnswer = managerAnswers.FirstOrDefault(m => m.Response == candidateAnswer.Response);

                if (managerAnswer != null)
                {
                    // Assigner le même QuestionId que celui du manager
                    candidateAnswer.QuestionId = managerAnswer.QuestionId;

                    // Ajouter la réponse du candidat à la base de données
                    var submittedAnswer = await _repository.AddAnswers(candidateAnswer);
                    submittedAnswers.Add(submittedAnswer);
                }
                else
                {
                    // Si aucune réponse correspondante n'est trouvée, vous pouvez lever une exception ou gérer autrement
                    throw new Exception($"No corresponding manager answer found for candidate response: {candidateAnswer.Response}");
                }
            }

            return submittedAnswers;
        }

        public async Task<List<Answercandidat>> GetSubmittedAnswersAsync(int testId)
        {
            return await _repository.GetSubmittedAnswersAsync(testId);
        }
        public async Task<List<Answercandidat>> GetAnswercandidatsByQuestionIdAsync(int questionId)
        {
            return await _repository.GetAnswercandidatsByQuestionIdAsync(questionId);
        }

        public async Task<int> CompareAndCalculateScoreAsync()
        {
            var candidateAnswers = await _repository.GetCandidateAnswersAsync();
            var managerAnswers = await _repository.GetManagerAnswersAsync();

            Console.WriteLine($"Nombre de réponses du candidat : {candidateAnswers.Count}");
            Console.WriteLine($"Nombre de réponses du manager : {managerAnswers.Count}");

            int score = 0;

            foreach (var candidateAnswer in candidateAnswers)
            {
                var correspondingManagerAnswer = managerAnswers.FirstOrDefault(a => a.Id == candidateAnswer.Id);


                if (correspondingManagerAnswer != null)
                {
                    if (candidateAnswer.IsCorrect == correspondingManagerAnswer.IsCorrect)
                    {
                        // Les réponses correspondent, attribuez un point
                        score++;
                    }
                    else
                    {
                        Console.WriteLine($"Mismatch - QuestionId: {candidateAnswer.QuestionId}");
                        Console.WriteLine($"Candidate IsCorrect: {candidateAnswer.IsCorrect}");
                        Console.WriteLine($"Manager IsCorrect: {correspondingManagerAnswer.IsCorrect}");
                    }
                }
                else
                {
                    Console.WriteLine($"No corresponding manager answer found for QuestionId: {candidateAnswer.QuestionId}");
                }
            }

            Console.WriteLine($"Score total : {score}");
            return score;
        }


        public async Task<int?> GetTestIdBySubjectAsync(string subject)
        {
            var test = await _context.Tests.FirstOrDefaultAsync(t => t.Subject == subject);
            return test?.Id;
        }

        public async Task UpdateFormAsync(Form form)
        {
            _context.Forms.Update(form);
            await _context.SaveChangesAsync();
        }





    }
}
