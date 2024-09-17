using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;

namespace App_plateforme_de_recurtement.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Dictionary<string, string> _autoResponses = new Dictionary<string, string>
        {
            { "statut candidature", "Pour vérifier le statut de votre candidature, veuillez consulter la section 'Mes Candidatures' dans votre espace personnel." },
            { "offres de stage", "Vous pouvez consulter toutes les offres de stage disponibles sur notre page 'Offres de Stage'." },
            { "processus de recrutement", "Le processus de recrutement comprend la soumission de votre candidature, une pré-sélection, et un entretien avec notre équipe." },
            { "candidatures reçues", "Vous pouvez consulter toutes les candidatures reçues dans la section 'Gestion des Candidatures' de votre tableau de bord." },
            { "outils de gestion des entretiens", "Nous offrons des outils pour planifier et suivre les entretiens directement depuis votre tableau de bord." }
        };

        public async Task SendMessage(string user, string message)
        {
            // Envoi le message de l'utilisateur
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            // Vérifie si le message correspond à une réponse automatique
            if (_autoResponses.ContainsKey(message.ToLower()))
            {
                string autoResponse = _autoResponses[message.ToLower()];
                await Clients.All.SendAsync("ReceiveMessage", "Bot", autoResponse);
            }
        }
    }
}
