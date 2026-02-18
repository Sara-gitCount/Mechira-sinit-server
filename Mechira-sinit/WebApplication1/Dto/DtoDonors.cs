using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dto
{
    public class DtoDonors
    {
        [Required, MinLength(2)]
        public string FirstName { get; set; }
        [Required, MinLength(2)]
        public string LastName { get; set; }
        public List<DtoGifts> Donations { get; set; }
        [Required, Phone(ErrorMessage = "הטלפון אינו תקין")]
        public string Phone { get; set; }
        [EmailAddress(ErrorMessage = "המייל אינו תקין")]
        public string Email { get; set; }

    }

    public class DonorCreateDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
