using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Dto
{
    public class DtoGifts
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [Required, Range(15, int.MaxValue, ErrorMessage = "המחיר חייב להיות גדול מ- 15")]
        public int Price { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int DonorId { get; set; }



    }
    public class DtoGifts_D
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
    public class DtoGift
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        [Range(15, int.MaxValue, ErrorMessage = "המכיר חייב להיות יותר מ 15 ₪")]
        public int Price { get; set; }
        [Required]
        public int DonorId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public Category Category { get; set; }
    }

    public class Basket
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        [Range(15, int.MaxValue, ErrorMessage = "המכיר חייב להיות יותר מ 15 ₪")]
        public int Price { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public int Amount { get; set; }
        public int giftId { get; set; }
    }
}
