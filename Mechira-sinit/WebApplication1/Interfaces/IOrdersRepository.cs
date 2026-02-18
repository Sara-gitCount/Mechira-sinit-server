using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IOrdersRepository
    {
        Task<List<IGrouping<Gift, Order>>> GetOrdersByGifts();
        Task<List<Gift>> GetGiftOrderByPrice();
        Task<List<Gift>> GetGiftOrderByOrders();
        Task<List<User>> GetAllUsers();
        Task<bool> CreateOrder(int userId, int giftId);
        Task<bool> DeleteOrder(int orderId);
        Task<bool> ChangeStatus(int orderId);
        //Task<List<Gift>> GetGiftsOrderByCategory(int categoryId);
    }
}
