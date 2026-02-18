using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly StoreContext context;
        public UsersRepository(StoreContext context)
        {
            this.context = context;
        }

        public async Task<List<Gift>> Basket(int idUser)
        {
            // Load all orders for the user including the Gift (and its Category if present)
            var orders = await context.orders
                .Include(o => o.Gift)
                    .ThenInclude(g => g.Category)
                .Where(o => o.UserId == idUser)
                .ToListAsync();

            // Group orders by Gift and return one Gift instance per group.
            // We populate the Gift.Orders list with only this user's orders so callers can get the amount via Gift.Orders.Count.
            var gifts = orders
                .GroupBy(o => o.GiftId)
                .Select(g =>
                {
                    var gift = g.First().Gift;
                    gift.Orders = g.ToList();
                    return gift;
                })
                .ToList();

            return gifts;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            context.useres.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await context.useres.FindAsync(id);
            if (user == null)
                return false;
            context.useres.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await context.useres.ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await context.useres.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await context.useres.FindAsync(id);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var u = await context.useres.FindAsync(user.Id);
            if (u == null)
                return null;
            u.FirstName = user.FirstName;
            u.Password = user.Password;
            u.LastName = user.LastName;
            u.Email = user.Email;
            u.Phone = user.Phone;
            u.Address = user.Address;
            await context.SaveChangesAsync();
            return u;
        }
    }
}
