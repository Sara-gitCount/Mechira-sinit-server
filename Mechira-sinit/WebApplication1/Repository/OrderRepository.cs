using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class OrderRepository: IOrdersRepository
    {
        private readonly StoreContext context;

        public OrderRepository(StoreContext context)
        {
            this.context = context;
        }

        public async Task<bool> ChangeStatus(int orderId)
        {
            var order = await context.orders.FindAsync(orderId);
            if (order == null)
                return false;
            order.Status = true;
            //context.orders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateOrder(int userId, int giftId)
        {
            var order = new Order
            {
                UserId = userId,
                GiftId = giftId
            };
            await context.orders.AddAsync(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrder(int orderId)
        {
            var order = await context.orders.FindAsync(orderId);
            
            if (order == null)
                return false;
            if (order.Status == false)
            { 
                context.orders.Remove(order);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await context.orders
                .Where(o => o.Status == true)
                .Select(o => o.User)
                .Distinct()
                .ToListAsync();
            return users;
        }

        public async Task<List<Gift>> GetGiftOrderByOrders()//המתנה הנרכשת ביותר
        {
            var gifts = await context.orders
                .Where(o =>o.Status == true) 
                .GroupBy(o => o.GiftId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.First().Gift)
                .ToListAsync();

            return gifts;
        }

        public async Task<List<Gift>> GetGiftOrderByPrice()//המתנה היקרה ביותר-מיון לפי
        {
            var gifts = await context.orders
                .Where(o => o.Status == true)
                .Select(o => o.Gift)
                .OrderBy(g => g.Price)
                .ToListAsync();
            return gifts;
        }

        public Task<List<Gift>> GetGiftsOrderByCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<IGrouping<Gift,Order>>> GetOrdersByGifts()//רכישות כרטיסים עבור כל מתנה
        {
            var orders =await context.orders
                .Include(o => o.Gift)
                .Include(o => o.User)
                .GroupBy(o => o.Gift)
                .ToListAsync();
            return orders;
        }
    }
}
