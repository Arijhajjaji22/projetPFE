namespace App_plateforme_de_recurtement.Models
{
    public class SavePdfRequest
    {
        public string? FileName { get; set; }
        public byte[]? PdfContent { get; set; }
        public string PdfData { get; set; } // Assurez-vous que PdfData est une chaîne pour le contenu Base64
        // Autres propriétés si nécessaires
    }
}
