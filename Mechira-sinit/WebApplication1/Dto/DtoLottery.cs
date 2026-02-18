using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dto
{
    public class DtoLottery
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string GiftName { get; set; }

    }
}
