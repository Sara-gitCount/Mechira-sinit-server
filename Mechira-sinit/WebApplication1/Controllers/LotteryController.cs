using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("lottery/[controller]")]
    public class LotteryController:ControllerBase
    {
        private readonly ILotteryService lotteryService;
        private readonly ILogger<LotteryController> logger;
        public LotteryController(ILotteryService lotteryService, ILogger<LotteryController> logger)
        {
            this.lotteryService = lotteryService;
            this.logger = logger;
        }

        [HttpGet]
        [Route("GetAllWinners")]
        public async Task<ActionResult<List<DtoLottery>>> GetAllWinners()
        {
            return await lotteryService.GetAllWinnersAsync();
        }
        
        [HttpGet]
        [Route("GetAllRevenue")]
        public async Task<ActionResult<int>> GetAllRevenue()
        {
            return await lotteryService.GetAllRevenue();
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Lottery_SendingEmail()
        {
            return await lotteryService.LotteryAsync();
        }

        //[HttpPost]
        //[Route("SendingEmail")]
        //public async Task<ActionResult<bool>> SendingEmail(User user, Gift gift)
        //{
        //    return await lotteryService.SendingEmail(user, gift);
        //}

    }
}
