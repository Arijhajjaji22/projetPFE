namespace App_plateforme_de_recurtement.Models
{
    public class PdfDocument
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Ou tout type d'ID d'utilisateur que vous utilisez
        public byte[] PdfData { get; set; } // Ajoutez cette propriété pour stocker les données PDF

        // D'autres propriétés si nécessaires
    }

}

