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
    public class UserControllerTests
    {
        [Fact]
        public async Task GetAllUsers_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IUsersService>();
            var list = new List<DtoUser> { new DtoUser { FirstName = "F", Email = "e@x" } };
            mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(list);

            var controller = new UserController(new Mock<ILogger<UserController>>().Object, mockService.Object);

            var action = await controller.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<List<DtoUser>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetUserById_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IUsersService>();
            var dto = new DtoUser { FirstName = "A", Email = "a@b" };
            mockService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(dto);

            var controller = new UserController(new Mock<ILogger<UserController>>().Object, mockService.Object);

            var action = await controller.GetUserById(1);

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<DtoUser>(ok.Value);
            Assert.Equal("A", value.FirstName);
        }

        [Fact]
        public async Task GetUserById_ServiceThrowsKeyNotFound_ReturnsNotFound()
        {
            var mockService = new Mock<IUsersService>();
            mockService.Setup(s => s.GetUserByIdAsync(2)).ThrowsAsync(new KeyNotFoundException("nf"));

            var controller = new UserController(new Mock<ILogger<UserController>>().Object, mockService.Object);

            var action = await controller.GetUserById(2);

            var notFound = Assert.IsType<NotFoundObjectResult>(action.Result);
            Assert.Contains("nf", notFound.Value?.ToString());
        }

        [Fact]
        public async Task GetUserById_ServiceThrowsArgument_ReturnsBadRequest()
        {
            var mockService = new Mock<IUsersService>();
            mockService.Setup(s => s.GetUserByIdAsync(3)).ThrowsAsync(new ArgumentException("bad id"));

            var controller = new UserController(new Mock<ILogger<UserController>>().Object, mockService.Object);

            var action = await controller.GetUserById(3);

            var bad = Assert.IsType<BadRequestObjectResult>(action.Result);
            Assert.Contains("bad id", bad.Value?.ToString());
        }

        [Fact]
        public async Task Basket_ServiceReturnsList_ReturnsList()
        {
            var mockService = new Mock<IUsersService>();
            var baskets = new List<Basket> { new Basket { Id = 1, Name = "x", Price = 20, CategoryName = "c" } };
            mockService.Setup(s => s.Basket(5)).ReturnsAsync(baskets);

            var controller = new UserController(new Mock<ILogger<UserController>>().Object, mockService.Object);

            var result = await controller.Basket(5);

            Assert.Equal(baskets, result);
        }
    }
}
