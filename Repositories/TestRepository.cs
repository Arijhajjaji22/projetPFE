using App_plateforme_de_recurtement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_plateforme_de_recurtement.Data.Repositories
{
    public class TestRepository
    {
        private readonly ApplicationDbContext _context;

        public TestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Test> AddTestAsync(Test test)
        {
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();
            return test;
        }

        public async Task<List<Answercandidat>> GetAnswercandidatsByTestIdAsync(int testId)
        {
            return await _context.answercandidats
                .Where(a => a.Id == testId)
                .ToListAsync();
        }

        public List<Test> GetAllTests()
        {
            return _context.Tests.ToList();
        }
        public List<Test> GetTestsBySubject(string subject)
        {
            return _context.Tests
                .Include(t => t.Questions)
                    .ThenInclude(q => q.Answers) // Inclure les réponses associées à chaque question
                .Where(t => t.Subject == subject)
                .ToList();
        }

        public async Task<Test> UpdateTestAsync(Test test)
        {
            var existingTest = await _context.Tests
                .Include(t => t.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(t => t.Id == test.Id);

            if (existingTest != null)
            {
                // Mettre à jour les valeurs du test existant
                _context.Entry(existingTest).CurrentValues.SetValues(test);

                // Mettre à jour les questions existantes et leurs réponses
                foreach (var updatedQuestion in test.Questions)
                {
                    var existingQuestion = existingTest.Questions.FirstOrDefault(q => q.Id == updatedQuestion.Id);
                    if (existingQuestion != null)
                    {
                        // Mettre à jour les valeurs de la question existante
                        _context.Entry(existingQuestion).CurrentValues.SetValues(updatedQuestion);

                        // Mettre à jour les réponses existantes de la question
                        foreach (var updatedAnswer in updatedQuestion.Answers)
                        {
                            var existingAnswer = existingQuestion.Answers.FirstOrDefault(a => a.Id == updatedAnswer.Id);
                            if (existingAnswer != null)
                            {
                                // Mettre à jour les valeurs de la réponse existante
                                _context.Entry(existingAnswer).CurrentValues.SetValues(updatedAnswer);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            return existingTest;
        }







        public async Task DeleteTestAsync(Test test)
        {
            // Récupérer les questions associées au test
            var questions = await _context.Question.Where(q => q.TestId == test.Id).ToListAsync();

            // Supprimer les questions associées
            foreach (var question in questions)
            {
                _context.Question.Remove(question);
            }

            // Ensuite, supprimer le test lui-même
            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
        }
        public async Task<Test> GetTestByIdAsync(int id)
        {
            return await _context.Tests
                .Include(t => t.Questions)
                    .ThenInclude(q => q.Answers)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<Question> AddQuestionAsync(Question question)
        {
            _context.Question.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<List<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _context.Question
                .Include(q => q.Answers)
                .Where(q => q.TestId == testId)
                .ToListAsync();
        }
        public async Task<Answer> AddAnswerAsync(Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<List<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            return await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            _context.Entry(answer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Answercandidat> AddAnswers(Answercandidat answer)
        {
            _context.answercandidats.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<List<Answercandidat>> GetSubmittedAnswersAsync(int Id)
        {
            return await _context.answercandidats
                .Where(a => a.Id == Id)
                .ToListAsync();
        }

        public async Task<List<Answercandidat>> GetAnswercandidatsByQuestionIdAsync(int questionId)
        {
            return await _context.answercandidats
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }

        public async Task<Answer> GetCorrectAnswerByQuestionIdAsync(int questionId)
        {
            return await _context.Answers.FirstOrDefaultAsync(a => a.QuestionId == questionId && a.IsCorrect);
        }
        public async Task<List<Answer>> GetManagerAnswersAsync()
        {
            return await _context.Answers.ToListAsync();
        }

        public async Task<List<Answercandidat>> GetCandidateAnswersAsync()
        {
            return await _context.answercandidats.ToListAsync();
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }



    }

}
