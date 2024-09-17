using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace App_plateforme_de_recurtement.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public string Response { get; set; }
        public bool IsCorrect { get; set; }
      public int QuestionId { get; set; }
 
      //  public Question Question { get; set; }
    }
}
