using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IOrdersService
    {
        Task<List<GiftOrderDto>> GetOrdersByGifts();
        Task<List<DtoGifts>> GetGiftOrderByPrice();
        Task<List<DtoGifts_D>> GetGiftOrderByOrders();
        Task<List<DtoUser>> GetAllUsers();
        Task<bool> CreateOrder(int userId, int giftId);
        Task<bool> DeleteOrder(int orderId);
        Task<bool> ChangeStatus(int orderId);
    }
}
