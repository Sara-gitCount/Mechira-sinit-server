using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface ILotteryRepository
    {
        Task<List<User>> LotteryAsync(Gift g);
        Task<List<Lottery>> GetAllWinnersAsync();
        Task<int> GetAllRevenue();
        //Task<bool> SendingEmail(Useres user,string gift);
        Task<bool> CreateLottery(int userId, int giftId);


    }
}
