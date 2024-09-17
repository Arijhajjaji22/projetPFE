namespace App_plateforme_de_recurtement.DTOs
{
    public class FormDto
    {
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Faculte { get; set; }
        public string NiveauEtudes { get; set; }
        public IFormFile CV { get; set; }
        public IFormFile LettreMotivation { get; set; }

        public int AdminStageOfferId { get; set; }
        public int TestId { get; set; }
    }
}
