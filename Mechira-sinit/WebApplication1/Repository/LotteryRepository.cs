using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class LotteryRepository: ILotteryRepository
    {
        private readonly StoreContext context;
        public LotteryRepository(StoreContext context)
        {
            this.context = context;
        }

        public async Task<bool> CreateLottery(int userId, int giftId)
        {
            Lottery l = new Lottery
            {
                GiftId = giftId,
                UserId = userId,
            };
            context.lotteries.Add(l);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetAllRevenue()
        {
            var reveue = await context.orders
                .SumAsync(o => o.Gift.Price);
            return reveue;
        }

        public Task<List<Lottery>> GetAllWinnersAsync()
        {
            var lotteries = context.lotteries.ToListAsync();
            return lotteries;
        }

        public async Task<List<User>> LotteryAsync(Gift g)
        {
            return await context.orders
                    .Where(o => o.Gift == g)
                    .Select(o => o.User)
                    .ToListAsync();

        }
    }
}
