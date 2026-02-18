using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Dto;
using WebApplication1.Interfaces;
using Xunit;

namespace WebApplication1.Tests.Controllers
{
    public class LotteryControllerTests
    {
        [Fact]
        public async Task GetAllWinners_ServiceReturns_ListOk()
        {
            var mockService = new Mock<ILotteryService>();
            var list = new List<DtoLottery> { new DtoLottery { GiftName = "G", UserName = "U" } };
            mockService.Setup(s => s.GetAllWinnersAsync()).ReturnsAsync(list);

            var controller = new LotteryController(mockService.Object, new Mock<ILogger<LotteryController>>().Object);

            var action = await controller.GetAllWinners();

            var ok = Assert.IsType<ActionResult<List<DtoLottery>>>(action);
            var value = Assert.IsType<List<DtoLottery>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetAllWinners_ServiceThrows_Propagates()
        {
            var mockService = new Mock<ILotteryService>();
            mockService.Setup(s => s.GetAllWinnersAsync()).ThrowsAsync(new KeyNotFoundException("no winners"));

            var controller = new LotteryController(mockService.Object, new Mock<ILogger<LotteryController>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => controller.GetAllWinners());
        }

        [Fact]
        public async Task GetAllRevenue_ServiceReturns_ValueOk()
        {
            var mockService = new Mock<ILotteryService>();
            mockService.Setup(s => s.GetAllRevenue()).ReturnsAsync(500);

            var controller = new LotteryController(mockService.Object, new Mock<ILogger<LotteryController>>().Object);

            var action = await controller.GetAllRevenue();

            var ok = Assert.IsType<ActionResult<int>>(action);
            Assert.Equal(500, ok.Value);
        }

        [Fact]
        public async Task Lottery_SendingEmail_ServiceReturnsTrue_ReturnsTrue()
        {
            var mockService = new Mock<ILotteryService>();
            mockService.Setup(s => s.LotteryAsync()).ReturnsAsync(true);

            var controller = new LotteryController(mockService.Object, new Mock<ILogger<LotteryController>>().Object);

            var action = await controller.Lottery_SendingEmail();

            var ok = Assert.IsType<ActionResult<bool>>(action);
            Assert.True(ok.Value);
        }

        [Fact]
        public async Task Lottery_SendingEmail_ServiceThrows_Propagates()
        {
            var mockService = new Mock<ILotteryService>();
            mockService.Setup(s => s.LotteryAsync()).ThrowsAsync(new Exception("boom"));

            var controller = new LotteryController(mockService.Object, new Mock<ILogger<LotteryController>>().Object);

            await Assert.ThrowsAsync<Exception>(() => controller.Lottery_SendingEmail());
        }
    }
}
