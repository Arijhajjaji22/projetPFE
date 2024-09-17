using System.Collections.Generic;
namespace App_plateforme_de_recurtement.Services

{
   

    public class MappingService
    {
        private Dictionary<int, int> _idMappings;

        public MappingService()
        {
            _idMappings = new Dictionary<int, int>();
        }

        // Ajouter une correspondance entre une ancienne et une nouvelle ID
        public void AddMapping(int oldId, int newId)
        {
            _idMappings[oldId] = newId;
        }

        // Obtenir la nouvelle ID correspondante à partir de l'ancienne ID
        public int GetNewId(int oldId)
        {
            if (_idMappings.ContainsKey(oldId))
            {
                return _idMappings[oldId];
            }
            else
            {
                // Si aucune correspondance n'est trouvée, retourner -1 ou une valeur par défaut
                return -1; // Ou une autre valeur par défaut selon la logique de votre application
            }
        }
    }

}
