using System.ComponentModel.DataAnnotations;

namespace App_plateforme_de_recurtement.Models
{
    public class EmailRequest
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
 
    }

}
