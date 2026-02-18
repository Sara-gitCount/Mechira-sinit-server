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
    public class GiftControllerTests
    {
        [Fact]
        public async Task GetGiftByDonor_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IGiftService>();
            var list = new List<DtoGifts_D> { new DtoGifts_D { Name = "g", Image = "i", CategoryId = 1 } };
            mockService.Setup(s => s.GetGiftByDonor("donor")).ReturnsAsync(list);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var action = await controller.GetGiftByDonor("donor");

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<List<DtoGifts_D>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetGiftByDonor_ServiceThrowsArgumentNull_Propagates()
        {
            var mockService = new Mock<IGiftService>();
            mockService.Setup(s => s.GetGiftByDonor(null!)).ThrowsAsync(new ArgumentNullException());

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => controller.GetGiftByDonor(null!));
        }

        [Fact]
        public async Task GetAllGifts_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IGiftService>();
            var list = new List<DtoGift> { new DtoGift { Id = 1, Name = "g", Image = "i", Price = 20, DonorId = 1, CategoryName = "c" } };
            mockService.Setup(s => s.GetGiftsAsync()).ReturnsAsync(list);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var action = await controller.GetAllGifts();

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<List<DtoGift>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetGiftByName_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IGiftService>();
            var dto = new DtoGift { Id = 2, Name = "x", Image = "i", Price = 15, DonorId = 1, CategoryName = "cat" };
            mockService.Setup(s => s.GetGiftByName("x")).ReturnsAsync(dto);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var action = await controller.GetGiftByName("x");

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<DtoGift>(ok.Value);
            Assert.Equal("x", value.Name);
        }

        [Fact]
        public async Task getByNumOfUsers_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IGiftService>();
            var list = new List<DtoGifts_D> { new DtoGifts_D { Name = "g" } };
            mockService.Setup(s => s.GetGiftByNOfUsers(5)).ReturnsAsync(list);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var action = await controller.getByNumOfUsers(5);

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<List<DtoGifts_D>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetDonor_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IGiftService>();
            var dto = new DtoDonors { FirstName = "A", Email = "e@x" };
            mockService.Setup(s => s.GetDonorsAsync(3)).ReturnsAsync(dto);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var action = await controller.GetDonor(3);

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<DtoDonors>(ok.Value);
            Assert.Equal("A", value.FirstName);
        }

        [Fact]
        public async Task addGift_ServiceSucceeds_ReturnsOk()
        {
            var mockService = new Mock<IGiftService>();
            mockService.Setup(s => s.AddGift(It.IsAny<DtoGifts>())).ReturnsAsync(true);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var result = await controller.addGift(new DtoGifts { Name = "n", CategoryId = 1, DonorId = 1, Price = 20, Image = "i" });

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task deleteGift_ServiceSucceeds_ReturnsOk()
        {
            var mockService = new Mock<IGiftService>();
            mockService.Setup(s => s.DeleteGiftAsync(4)).ReturnsAsync(true);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var result = await controller.deleteGift(4);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task updateGift_ServiceSucceeds_ReturnsOk()
        {
            var mockService = new Mock<IGiftService>();
            mockService.Setup(s => s.UpdateGiftAsync(It.IsAny<DtoGifts>(), 7)).ReturnsAsync(true);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var result = await controller.updateGift(new DtoGifts { Name = "u", CategoryId = 1, DonorId = 1, Price = 20, Image = "i" }, 7);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetOrderByPrice_Category_ReturnsOk()
        {
            var mockService = new Mock<IGiftService>();
            var list = new List<DtoGift> { new DtoGift { Id = 1, Name = "g", Price = 20, DonorId = 1, CategoryName = "c", Image = "i" } };
            mockService.Setup(s => s.GetOrderByPrice_Category()).ReturnsAsync(list);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var action = await controller.GetOrderByPrice_CategoryAsync();

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<List<DtoGift>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetById_ReturnsDtoGifts()
        {
            var mockService = new Mock<IGiftService>();
            var dto = new DtoGifts { Name = "n", CategoryId = 2, DonorId = 3, Price = 30, Image = "i" };
            mockService.Setup(s => s.GetGiftById(9)).ReturnsAsync(dto);

            var controller = new GiftControler(mockService.Object, new Mock<ILogger<GiftControler>>().Object);

            var result = await controller.GetById(9);

            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.CategoryId, result.CategoryId);
        }
    }
}
