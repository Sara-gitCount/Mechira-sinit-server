using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress(ErrorMessage = "המייל אינו תקין")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }
        [Required]
        public string Roles {  get; set; }
    }
}
