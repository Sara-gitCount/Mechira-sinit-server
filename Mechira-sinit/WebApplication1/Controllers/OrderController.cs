using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("orders/[controller]")]
    public class OrderController: Controller
    {
        private readonly IOrdersService orderService;
        private readonly ILogger<OrderController> logger;
        public OrderController(IOrdersService orderService,ILogger<OrderController> logger)
            {
                this.orderService = orderService;
                this.logger = logger;
        }

        [HttpGet]
        [Route("getAllUsers")]
        //[Authorize(Roles ="manager")]
        public async Task<ActionResult<List<DtoUser>>> getAllUsers()
        {
            try
            {
                return await orderService.GetAllUsers();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("getGiftOrderByOrders")]
        //[Authorize(Roles ="manager")]
        public async Task<ActionResult<List<DtoGifts_D>>> GetGiftOrderByOrders()
        {
            try
            {
                return await orderService.GetGiftOrderByOrders();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("getOrderByPrice")]
        //[Authorize]
        public async Task<ActionResult<List<DtoGifts>>> getOrderByPrice()
        {
            try
            {
                return await orderService.GetGiftOrderByPrice();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("createOrder/{userId}/{giftId}")]
        [Authorize]
        public async Task<ActionResult<bool>> createOrder(int userId, int giftId)
        {
            try
            {
                return await orderService.CreateOrder(userId, giftId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        [Route("deleteOrder/${orderId}")]
        [Authorize]
        public async Task<ActionResult<bool>> deleteOrder(int orderId)
        {
            try
            {
                return await orderService.DeleteOrder(orderId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        [Route("changeStatus/${orderId}")]
        public async Task<ActionResult<bool>> changeStatus(int orderId)
        {
            try
            {
                return await orderService.ChangeStatus(orderId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("getOrderByGift")]
        //[Authorize(Roles = "manager")]
        public async Task<List<GiftOrderDto>> getOrderByGift()
        {
            var orders = await orderService.GetOrdersByGifts();
            if (orders == null)
            {
                logger.LogWarning("orders are empty");
            }
            return orders;
        }

    }
}
