using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App_plateforme_de_recurtement.Services
{
    public class FormService
    {
        private readonly FormRepository _formRepository;

        public FormService(FormRepository formRepository)
        {
            _formRepository = formRepository;
        }
        public async Task UpdateInterviewPassedAsync(int id, bool passed)
        {
            await _formRepository.UpdateInterviewPassedAsync(id, passed);
        }
        public async Task<IEnumerable<Form>> GetAllFormsAsync()
        {
            return await _formRepository.GetAllFormsAsync();
        }

        public async Task<Form> GetFormByIdAsync(int id)
        {
            return await _formRepository.GetFormByIdAsync(id);
        }

        public async Task<Form> AddFormAsync(Form form)
        {
            return await _formRepository.AddFormAsync(form);
        }

        public async Task<Form> UpdateFormAsync(Form form)
        {
            return await _formRepository.UpdateFormAsync(form);
        }

        public async Task DeleteFormAsync(int id)
        {
            await _formRepository.DeleteFormAsync(id);
        }
        public async Task<Form> UpdateAdminStageOfferIdAsync(int formId, int adminStageOfferId)
        {
            var form = await _formRepository.GetFormByIdAsync(formId);
            if (form == null)
            {
                return null;
            }

            form.AdminStageOfferId = adminStageOfferId;
            await _formRepository.UpdateFormAsync(form);
            return form;
        }
        public async Task<IEnumerable<Form>> GetFormsByStageOfferIdAsync(int stageOfferId)
        {
            return await _formRepository.GetFormsByStageOfferIdAsync(stageOfferId);
        }
        public async Task<int> CountFormsByStageOfferIdAsync(int stageOfferId)
        {
            return await _formRepository.CountFormsByStageOfferIdAsync(stageOfferId);
        }
        public async Task UpdateStarRatingAsync(int formId, int starRating)
        {
            await _formRepository.UpdateStarRatingAsync(formId, starRating);
        }
        public async Task<bool> UpdateTestScheduleAsync(RescheduleRequesttest request)
        {
            if (!request.UserId.HasValue)
            {
                // Si UserId est null, retournez false
                return false;
            }

            var form = await _formRepository.GetFormByUserIdAsync(request.UserId.Value);
            if (form == null)
            {
                // Si aucun formulaire n'est trouvé avec cet ID utilisateur, retournez false
                return false;
            }

            // Aucune mise à jour de ScheduledDate dans cette méthode
            // Ajoutez toute autre logique nécessaire pour la mise à jour du test

            return true;
        }


    }
}
