using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Repository;

namespace WebApplication1.Services
{
    public class OrderService: IOrdersService
    {
        private readonly IOrdersRepository orderRepository;
        private readonly ILogger<OrderService> logger;
        public OrderService(IOrdersRepository orederRepository, ILogger<OrderService> logger)
        {
            this.orderRepository = orederRepository;
            this.logger = logger;
        }

        public async Task<List<DtoUser>> GetAllUsers()
        {
            var gifts = await orderRepository.GetAllUsers();
            if (gifts == null || !gifts.Any())
            {
                logger.LogWarning("No users found in the database.");
            }
            return gifts.Select(MapToResponseDto).ToList();

        }

        public async Task<List<DtoGifts_D>> GetGiftOrderByOrders()
        {
            var gifts = await orderRepository.GetGiftOrderByOrders();
            if (gifts == null || !gifts.Any())
            {
                logger.LogWarning("No gifts found in the database.");
            }
            return gifts.Select(MapToResponseDto_d).ToList();

        }

        public async Task<List<DtoGifts>> GetGiftOrderByPrice()
        {
            var gifts = await orderRepository.GetGiftOrderByPrice();
            if (gifts == null || !gifts.Any())
            {
                logger.LogWarning("No gifts found in the database.");
            }
            return gifts.Select(MapToResponseDto).ToList();
        }

        public async Task<List<GiftOrderDto>> GetOrdersByGifts()
        {
            var orders = await orderRepository.GetOrdersByGifts();
            if(orders == null)
            {
                logger.LogWarning("No orders found in the database");
            }
            return orders.Select(g => new GiftOrderDto
            {
                GiftName = g.Key.Name,
                Users = g
               .Select(o => o.User.FirstName + " " + o.User.LastName)
               .ToList()
            }).ToList();
        }
        public async Task<bool> CreateOrder(int userId, int giftId)
        {
            if (userId <= 0 || giftId <= 0)
            {
                logger.LogError("Invalid userId or giftId: userId={UserId}, giftId={GiftId}", userId, giftId);
                throw new ArgumentException("Invalid userId or giftId");
            }
            var result = await orderRepository.CreateOrder(userId, giftId);
            if (!result)
            {
                logger.LogError("Order could not be created for userId={UserId}, giftId={GiftId}", userId, giftId);
                throw new KeyNotFoundException("Order could not be created");
            }
            logger.LogInformation("Order created successfully for userId={UserId}, giftId={GiftId}", userId, giftId);
            return result;
        }

        public async Task<bool> DeleteOrder(int orderId)
        {
            if (orderId <= 0)
            {
                logger.LogError("Invalid orderId: {OrderId}", orderId);
                throw new ArgumentException("Invalid orderId");
            }
            var result = await orderRepository.DeleteOrder(orderId);
            if (!result)
            {
                logger.LogError("Order could not be deleted for orderId={OrderId}", orderId);
                throw new KeyNotFoundException("Order could not be deleted");
            }
            logger.LogInformation("Order deleted successfully for orderId={OrderId}", orderId);
            return result;
        }

        public async Task<bool> ChangeStatus(int orderId)
        {
            if (orderId <= 0)
            {
                logger.LogError("Invalid orderId: {OrderId}", orderId);
                throw new ArgumentException("Invalid orderId");
            }
            var result =await orderRepository.ChangeStatus(orderId);
            if (!result)
            {
                logger.LogError("Order status could not be changed for orderId={OrderId}", orderId);
                throw new KeyNotFoundException("Order status could not be changed");
            }
            logger.LogInformation("Order status changed successfully for orderId={OrderId}", orderId);
            return result;
        }

        private static DtoGifts MapToResponseDto(Gift gifts) => new DtoGifts
        {
            Name = gifts.Name,
            Description = gifts.Description,
            Image = gifts.Image,
            CategoryId = gifts.CategoryId,
            DonorId = gifts.DonorId,
            Price = gifts.Price,
        };
        private static DtoGifts_D MapToResponseDto_d(Gift gifts) => new DtoGifts_D
        {
            Name = gifts.Name,
            Description = gifts.Description,
            Image = gifts.Image,
            CategoryId = gifts.CategoryId,
        };
        private static DtoUser MapToResponseDto(User users) => new DtoUser
        {
            FirstName = users.FirstName,
            LastName = users.LastName,
            Phone = users.Phone,
            Email = users.Email,
        };
    }
}
