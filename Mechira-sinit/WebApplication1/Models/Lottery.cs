using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Lottery
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public int GiftId { get; set; }
        [Required]
        public Gift Gift { get; set; }

    }
}
