using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("gift/[controller]")]
    public class GiftControler: Controller
    {
        private readonly IGiftService giftService;
        private readonly ILogger<GiftControler> logger;
        public GiftControler(IGiftService giftService,ILogger<GiftControler> logger)
        {
            this.giftService = giftService;
            this.logger = logger;
        }

        [HttpGet]
        [Route("getByDonor")]
        public async Task<ActionResult<List<DtoGifts_D>>> GetGiftByDonor(string donorName)
        {
            var gifts = await giftService.GetGiftByDonor(donorName);
            return Ok(gifts);
        }

        [HttpGet]
        public async Task<ActionResult<List<DtoGift>>> GetAllGifts()
        {
            var gifts = await giftService.GetGiftsAsync();
            return Ok(gifts);
        }

        [HttpGet]
        [Route("getByName/{name}")]
        public async Task<ActionResult<DtoGift>> GetGiftByName(string name)
        {
            var gift = await giftService.GetGiftByName(name);
            return Ok(gift);
        }

        [HttpGet]
        [Route("GetByNumOfUsers")]
        public async Task<ActionResult<List<DtoGifts_D>>> getByNumOfUsers(int numOfUsers)
        {
            var gifts = await giftService.GetGiftByNOfUsers(numOfUsers);
            return Ok(gifts);
        }

        [HttpGet]
        [Route("getDonor/{giftId}")]
        public async Task<ActionResult<DtoDonors>> GetDonor(int giftId)
        {
            var donor = await giftService.GetDonorsAsync(giftId);
            return Ok(donor);
        }

        [HttpPost]
    
        public async Task<ActionResult> addGift(DtoGifts gift)
        {
            var g = await giftService.AddGift(gift);
            return Ok(new { success = true, message = "succed" });
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> deleteGift(int id)
        {
            var g = await giftService.DeleteGiftAsync(id);
            return Ok(new { success = true, message = "succed" });
        }

        [HttpPut]
        [Route("updale/{id}")]
        public async Task<ActionResult> updateGift(DtoGifts g, int id)
        {
            var gift = await giftService.UpdateGiftAsync(g, id);
            return Ok(new { success = true, message = "succed" });
        }

        [HttpGet]
        [Route("GetOrderByPrice_Category")]
        public async Task<ActionResult<List<DtoGift>>> GetOrderByPrice_CategoryAsync()
        {
            var gifts = await giftService.GetOrderByPrice_Category();
            return Ok(gifts);
        }

        [HttpGet]
        [Route("getById${id}")]
        public async Task<DtoGifts> GetById(int id)
        {
            var g = await giftService.GetGiftById(id);
            return g;
        }

    }
}
