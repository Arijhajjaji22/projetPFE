namespace App_plateforme_de_recurtement.DTOs
{
    public class TestReportDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public string Report { get; set; }
        public string Email { get; set; } // Ajout de la propriété Email
    }
}
