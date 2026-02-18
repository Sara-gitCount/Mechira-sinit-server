using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class GiftRepository:IGiftRepository
    {
        private readonly StoreContext context;

        public GiftRepository(StoreContext context)
        {
            this.context = context;
        }

        public async Task<bool> AddGift(Gift gift)
        {
            context.gifts.Add(gift);
            await context.SaveChangesAsync();
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteGiftAsync(int id)
        {
            var g = await context.gifts.FindAsync(id);
            if (g == null)
            {
              return false;
            }
            if(context.orders.Any(o => o.GiftId == id))
            {
                return false;
            }
            context.gifts.Remove(g);
            await context.SaveChangesAsync();
            return true;

        }

        public async Task<Donor> GetDonorsAsync(int idGift)
        {
            var gift = await context.gifts
                .Where(g => g.Id == idGift)
                .Select(g => g.Donor)
                .FirstOrDefaultAsync();
            if (gift != null)
                return gift;
            else
                return null;
        }

        public async Task<List<Gift>> GetGiftByDonor(string donorName)
        {
            var gifts = await context.gifts
               .Where(g => (g.Donor.FirstName + " " + g.Donor.LastName) == donorName)
               .ToListAsync();
            return gifts;
        }

        public async Task<Gift> GetGiftByName(string name)
        {
            var gift = await context.gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Include(g => g.Orders)
                .FirstOrDefaultAsync(g => g.Name == name);
            return gift;
        }

        public async Task<List<Gift>> GetGiftByNOfUsers(int nOfUsers)
        {
            var gifts = await context.orders
                .GroupBy(o => o.GiftId)
                .Where(g => g.Count() == nOfUsers)
                .Select(g => g.First().Gift)
                .ToListAsync();

            return gifts;
        }

        public async Task<List<Gift>> GetGiftsAsync()
        {
            return await context.gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Include(g => g.Orders)
                .ToListAsync();
        }

        public async Task<bool> UpdateGiftAsync(Gift gift)
        {
            var g = await context.gifts.FindAsync(gift.Id);
            if (g != null)
            {
                context.Entry(g).CurrentValues.SetValues(gift);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<Gift> GetById(int id)
        {
            var g = await context.gifts.FindAsync(id);
            return g;
        }

        public async Task<List<Gift>> GetOrderByPrice_Category()
        {
            return await context.gifts
                .Include(g => g.Category)
                .OrderBy(g => g.Price)
                .ThenBy(g => g.Category.Name)
                .ToListAsync();
        }
    }
}
