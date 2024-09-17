using System.ComponentModel.DataAnnotations;

namespace App_plateforme_de_recurtement.Models
{
    public class Answercandidat
    {
        [Key]
        public int Id { get; set; }
        public string Response { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }

    }
}
