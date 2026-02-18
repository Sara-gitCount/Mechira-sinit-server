using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dto
{
    public class DtoUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress(ErrorMessage = "המייל אינו תקין")]
        public string Email { get; set; }
        [Required, Phone]
        public string Phone { get; set; }
    }
}
public class UserCreateDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string Roles {  get; set; } 

}


//public class UserResponseDto
//{
//    public int Id { get; set; }
//    public string FirstName { get; set; } = string.Empty;
//    public string LastName { get; set; } = string.Empty;
//    public string Email { get; set; } = string.Empty;
//    public int Phone { get; set; }
//    public string Address { get; set; } = string.Empty;
    
//}