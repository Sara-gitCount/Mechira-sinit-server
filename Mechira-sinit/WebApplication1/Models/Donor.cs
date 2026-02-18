using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Donor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public List<DtoGifts_D> Donations { get; set; }
        public List<Gift> Donations { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

    }
}
