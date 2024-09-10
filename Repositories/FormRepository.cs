using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App_plateforme_de_recurtement.Repositories
{
    public class FormRepository
    {
        private readonly ApplicationDbContext _context;

        public FormRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Form>> GetAllFormsAsync()
        {
            return await _context.Forms.ToListAsync();
        }

        public async Task<Form> GetFormByIdAsync(int id)
        {
            return await _context.Forms.FindAsync(id);
        }
        public async Task<Form> GetFormByUserIdAsync(int userId)
        {
            return await _context.Forms
                                  .Where(f => f.UserId == userId)
                                  .FirstOrDefaultAsync();
        }
        public async Task<Form> AddFormAsync(Form form)
        {
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            return form;
        }

        public async Task<Form> UpdateFormAsync(Form form)
        {
            _context.Forms.Update(form);
            await _context.SaveChangesAsync();
            return form;
        }

        public async Task DeleteFormAsync(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form != null)
            {
                _context.Forms.Remove(form);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Form>> GetFormsByStageOfferIdAsync(int stageOfferId)
        {
            return await _context.Forms
                .Where(f => f.AdminStageOfferId == stageOfferId) // Filtrer les candidats rejetés
                .ToListAsync();
        }
        public async Task<int?> GetFormRatingAsync(int formId)
        {
            var form = await _context.Forms.FindAsync(formId);
            return form?.StarRating;
        }
        public async Task<int> CountFormsByStageOfferIdAsync(int stageOfferId)
        {
            return await _context.Forms
                .CountAsync(f => f.AdminStageOfferId == stageOfferId);
        }

        public async Task UpdateStarRatingAsync(int formId, int starRating)
        {
            var form = await GetFormByIdAsync(formId);
            if (form != null)
            {
                form.StarRating = starRating;
                _context.Forms.Update(form);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateInterviewPassedAsync(int id, bool passed)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form != null)
            {
                form.InterviewPassed = passed;
                _context.Forms.Update(form);
                await _context.SaveChangesAsync();
            }
        }

    }
}
