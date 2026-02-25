using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using Xunit;

namespace WebApplication1.Tests.Controllers
{
    public class OrderControllerTests
    {
        [Fact]
        public async Task GetAllUsers_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IOrdersService>();
            var list = new List<DtoUser> { new DtoUser { FirstName = "U", Email = "u@e" } };
            mockService.Setup(s => s.GetAllUsers()).ReturnsAsync(list);

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.getAllUsers();

            var ok = Assert.IsType<ActionResult<List<DtoUser>>>(action);
            var value = Assert.IsType<List<DtoUser>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetAllUsers_ServiceThrowsKeyNotFound_ReturnsNotFound()
        {
            var mockService = new Mock<IOrdersService>();
            mockService.Setup(s => s.GetAllUsers()).ThrowsAsync(new KeyNotFoundException("no users"));

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.getAllUsers();

            var notFound = Assert.IsType<NotFoundObjectResult>(action.Result);
            Assert.Contains("no users", notFound.Value?.ToString());
        }

        [Fact]
        public async Task GetGiftOrderByOrders_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IOrdersService>();
            var list = new List<DtoGifts_D> { new DtoGifts_D { Name = "g", CategoryId = 1, Image = "i" } };
            mockService.Setup(s => s.GetGiftOrderByOrders()).ReturnsAsync(list);

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.GetGiftOrderByOrders();

            var ok = Assert.IsType<ActionResult<List<DtoGifts_D>>>(action);
            var value = Assert.IsType<List<DtoGifts_D>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task getOrderByPrice_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IOrdersService>();
            var list = new List<DtoGifts> { new DtoGifts { Name = "g", CategoryId = 1, DonorId = 2, Price = 20, Image = "i" } };
            mockService.Setup(s => s.GetGiftOrderByPrice()).ReturnsAsync(list);

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.getOrderByPrice();

            var ok = Assert.IsType<ActionResult<List<DtoGifts>>>(action);
            var value = Assert.IsType<List<DtoGifts>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task createOrder_InvalidArgument_ReturnsBadRequest()
        {
            var mockService = new Mock<IOrdersService>();
            mockService.Setup(s => s.CreateOrder(0, 1)).ThrowsAsync(new ArgumentException("invalid"));

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.createOrder(0, 1);

            var bad = Assert.IsType<BadRequestObjectResult>(action.Result);
            Assert.Contains("invalid", bad.Value?.ToString());
        }

        [Fact]
        public async Task createOrder_NotFound_ReturnsNotFound()
        {
            var mockService = new Mock<IOrdersService>();
            mockService.Setup(s => s.CreateOrder(1, 2)).ThrowsAsync(new KeyNotFoundException("nf"));

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.createOrder(1, 2);

            var notFound = Assert.IsType<NotFoundObjectResult>(action.Result);
            Assert.Contains("nf", notFound.Value?.ToString());
        }

        [Fact]
        public async Task createOrder_Success_ReturnsTrue()
        {
            var mockService = new Mock<IOrdersService>();
            mockService.Setup(s => s.CreateOrder(1, 2)).ReturnsAsync(true);

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.createOrder(1, 2);

            var ok = Assert.IsType<ActionResult<bool>>(action);
            Assert.True(ok.Value);
        }

        [Fact]
        public async Task deleteOrder_InvalidArgument_ReturnsBadRequest()
        {
            var mockService = new Mock<IOrdersService>();
            mockService.Setup(s => s.DeleteOrder(0)).ThrowsAsync(new ArgumentException("bad"));

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.deleteOrder(0);

            var bad = Assert.IsType<BadRequestObjectResult>(action.Result);
            Assert.Contains("bad", bad.Value?.ToString());
        }

        [Fact]
        public async Task deleteOrder_NotFound_ReturnsNotFound()
        {
            var mockService = new Mock<IOrdersService>();
            mockService.Setup(s => s.DeleteOrder(5)).ThrowsAsync(new KeyNotFoundException("nf"));

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.deleteOrder(5);

            var notFound = Assert.IsType<NotFoundObjectResult>(action.Result);
            Assert.Contains("nf", notFound.Value?.ToString());
        }

        [Fact]
        public async Task changeStatus_InvalidArgument_ReturnsBadRequest()
        {
            var mockService = new Mock<IOrdersService>();
            mockService.Setup(s => s.ChangeStatus(0)).ThrowsAsync(new ArgumentException("bad"));

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.changeStatus(0);

            var bad = Assert.IsType<BadRequestObjectResult>(action.Result);
            Assert.Contains("bad", bad.Value?.ToString());
        }

        [Fact]
        public async Task changeStatus_NotFound_ReturnsNotFound()
        {
            var mockService = new Mock<IOrdersService>();
            mockService.Setup(s => s.ChangeStatus(7)).ThrowsAsync(new KeyNotFoundException("nf"));

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var action = await controller.changeStatus(7);

            var notFound = Assert.IsType<NotFoundObjectResult>(action.Result);
            Assert.Contains("nf", notFound.Value?.ToString());
        }

        [Fact]
        public async Task getOrderByGift_ServiceReturns_ReturnsList()
        {
            var mockService = new Mock<IOrdersService>();
            var list = new List<GiftOrderDto> { new GiftOrderDto { GiftName = "G", Users = new List<string> { "A B" } } };
            mockService.Setup(s => s.GetOrdersByGifts()).ReturnsAsync(list);

            var controller = new OrderController(mockService.Object, new Mock<ILogger<OrderController>>().Object);

            var result = await controller.getOrderByGift();

            Assert.Equal(list, result);
        }
    }
}
