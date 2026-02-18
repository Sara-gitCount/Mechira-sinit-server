using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dto
{
    public class DtoOrders
    {
        [Required]
        public string NameGift { get; set; }
        public int NumOfOrders { get; set; }//number of orders

    }

    public class GiftOrderDto
    {
        public string GiftName {  get; set; }
        public List<string> Users { get; set; }
    }
}
