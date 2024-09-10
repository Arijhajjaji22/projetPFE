namespace App_plateforme_de_recurtement.DTOs
{
    public class InterviewConfirmationDto
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int AvailabilityId { get; set; }
        public DateTime EventDate { get; set; } // Assurez-vous que cette propriété correspond à votre besoin
    }
}
