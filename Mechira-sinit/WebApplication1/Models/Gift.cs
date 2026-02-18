using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Gift
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int DonorId { get; set; }
        [Required]
        public Donor Donor { get; set; }
        [Required]
        public string Image { get; set; }
        [Required, Range(15, int.MaxValue, ErrorMessage = "המחיר חייב להיות גדול מ- 15")]
        public int Price { get; set; }
        public List<Order> Orders { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
