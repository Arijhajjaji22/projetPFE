namespace App_plateforme_de_recurtement.Models
{
    public class UserRegistrationTemp
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
        public bool IsArchived { get; set; }
        public string Username { get; set; }
        public string ValidationToken { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
