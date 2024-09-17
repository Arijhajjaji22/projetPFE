using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace App_plateforme_de_recurtement.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public string Query { get; set; }
        public string Type { get; set; }
        public ICollection<Answer> Answers { get; set; }
       public int TestId { get; set; }

        // public Test Test { get; set; }
        public Question()
        {
            Answers = new List<Answer>();
        }
   
    }
  
}
