using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace App_plateforme_de_recurtement.Models
{
    public class Test
    {
        [Key]
        public int Id { get; set; }
      
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
    
        public string Subject { get; set; }

        public ICollection<Question> Questions { get; set; }
        public ICollection<Form>? Forms { get; set; }
        
    }
}
