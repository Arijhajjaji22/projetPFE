namespace App_plateforme_de_recurtement.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; } // Propriété liant la notification à un utilisateur
                                        // Autres propriétés pertinentes
    }

}
