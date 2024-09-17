using UglyToad.PdfPig;

namespace App_plateforme_de_recurtement.Services
{
    public class DocumentValidator
    {
        private static readonly string[] CvKeywordsEn = {
            "personal information", "contact information", "experience", "work experience",
            "education", "qualifications", "skills", "certifications", "professional summary",
            "objective", "references", "achievements", "projects", "languages", "internships",
            "awards", "training", "portfolio", "curriculum vitae"
        };

        private static readonly string[] CvKeywordsFr = {
            "informations personnelles", "coordonnées", "expérience", "expérience professionnelle",
            "éducation", "qualifications", "compétences", "certifications", "résumé professionnel",
            "objectif", "références", "réalisations", "projets", "langues", "stages",
            "prix", "formation", "portefeuille", "curriculum vitae"
        };

        private static readonly string[] CoverLetterKeywordsEn = {
            "dear", "hiring manager", "position", "application", "skills", "experience",
            "sincerely", "thank you", "cover letter", "motivation"
        };

        private static readonly string[] CoverLetterKeywordsFr = {
            "cher", "responsable du recrutement", "poste", "candidature", "compétences",
            "expérience", "cordialement", "merci", "lettre de motivation", "motivation"
        };

        public bool IsPdfCv(string filePath)
        {
            using var document = PdfDocument.Open(filePath);
            var text = string.Join(" ", document.GetPages().Select(page => page.Text).ToArray());

            // Vérifie si le texte contient des mots-clés typiques d'un CV en anglais ou en français
            return CvKeywordsEn.Any(keyword => text.ToLower().Contains(keyword.ToLower())) ||
                   CvKeywordsFr.Any(keyword => text.ToLower().Contains(keyword.ToLower()));
        }

        public bool IsPdfCoverLetter(string filePath)
        {
            using var document = PdfDocument.Open(filePath);
            var text = string.Join(" ", document.GetPages().Select(page => page.Text).ToArray());

            // Vérifie si le texte contient des mots-clés typiques d'une lettre de motivation en anglais ou en français
            return CoverLetterKeywordsEn.Any(keyword => text.ToLower().Contains(keyword.ToLower())) ||
                   CoverLetterKeywordsFr.Any(keyword => text.ToLower().Contains(keyword.ToLower()));
        }
    }
}
