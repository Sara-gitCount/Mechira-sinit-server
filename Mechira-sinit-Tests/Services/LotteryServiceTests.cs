using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;
using Xunit;

namespace WebApplication1.Tests.Services
{
    public class LotteryServiceTests
    {
        [Fact]
        public async Task GetAllRevenue_Success_ReturnsValue()
        {
            var mockLotteryRepo = new Mock<ILotteryRepository>();
            mockLotteryRepo.Setup(r => r.GetAllRevenue()).ReturnsAsync(123);

            var service = new LotteryService(mockLotteryRepo.Object, new Mock<IGiftRepository>().Object, new Mock<ILogger<LotteryService>>().Object);

            var result = await service.GetAllRevenue();

            Assert.Equal(123, result);
        }

        [Fact]
        public async Task GetAllRevenue_RepoThrows_PropagatesException()
        {
            var mockLotteryRepo = new Mock<ILotteryRepository>();
            mockLotteryRepo.Setup(r => r.GetAllRevenue()).ThrowsAsync(new InvalidOperationException("fail"));

            var service = new LotteryService(mockLotteryRepo.Object, new Mock<IGiftRepository>().Object, new Mock<ILogger<LotteryService>>().Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetAllRevenue());
        }

        [Fact]
        public async Task GetAllWinnersAsync_RepoReturnsMappedDtos()
        {
            var mockLotteryRepo = new Mock<ILotteryRepository>();
            var winners = new List<Lottery>
            {
                new Lottery { Gift = new Gift { Name = "G1" }, User = new User { FirstName = "A", LastName = "B" } }
            };
            mockLotteryRepo.Setup(r => r.GetAllWinnersAsync()).ReturnsAsync(winners);

            var service = new LotteryService(mockLotteryRepo.Object, new Mock<IGiftRepository>().Object, new Mock<ILogger<LotteryService>>().Object);

            var result = await service.GetAllWinnersAsync();

            Assert.Single(result);
            Assert.Equal("G1", result.First().GiftName);
            Assert.Equal("A B", result.First().UserName);
        }

        [Fact]
        public async Task GetAllWinnersAsync_RepoReturnsNull_ThrowsKeyNotFoundException()
        {
            var mockLotteryRepo = new Mock<ILotteryRepository>();
            mockLotteryRepo.Setup(r => r.GetAllWinnersAsync()).ReturnsAsync((List<Lottery>?)null);

            var service = new LotteryService(mockLotteryRepo.Object, new Mock<IGiftRepository>().Object, new Mock<ILogger<LotteryService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetAllWinnersAsync());
        }

        [Fact]
        public async Task LotteryAsync_NoGifts_ThrowsKeyNotFoundException()
        {
            var mockGiftRepo = new Mock<IGiftRepository>();
            mockGiftRepo.Setup(r => r.GetGiftsAsync()).ReturnsAsync(new List<Gift>());

            var service = new LotteryService(new Mock<ILotteryRepository>().Object, mockGiftRepo.Object, new Mock<ILogger<LotteryService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.LotteryAsync());
        }

        [Fact]
        public async Task LotteryAsync_GiftsExist_UsersNullFromRepo_ThrowsKeyNotFoundException()
        {
            var gift = new Gift { Id = 1, Name = "g" };
            var mockGiftRepo = new Mock<IGiftRepository>();
            mockGiftRepo.Setup(r => r.GetGiftsAsync()).ReturnsAsync(new List<Gift> { gift });

            var mockLotteryRepo = new Mock<ILotteryRepository>();
            mockLotteryRepo.Setup(r => r.LotteryAsync(gift)).ReturnsAsync((List<User>?)null);

            var service = new LotteryService(mockLotteryRepo.Object, mockGiftRepo.Object, new Mock<ILogger<LotteryService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.LotteryAsync());
        }

        [Fact]
        public async Task LotteryAsync_GiftsExist_UsersEmpty_CompletesSuccessfully()
        {
            var gift = new Gift { Id = 2, Name = "g2" };
            var mockGiftRepo = new Mock<IGiftRepository>();
            mockGiftRepo.Setup(r => r.GetGiftsAsync()).ReturnsAsync(new List<Gift> { gift });

            var mockLotteryRepo = new Mock<ILotteryRepository>();
            mockLotteryRepo.Setup(r => r.LotteryAsync(gift)).ReturnsAsync(new List<User>());

            var service = new LotteryService(mockLotteryRepo.Object, mockGiftRepo.Object, new Mock<ILogger<LotteryService>>().Object);

            var result = await service.LotteryAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task LotteryAsync_WinnerIsNull_ReturnsFalse()
        {
            var gift = new Gift { Id = 3, Name = "g3" };
            var mockGiftRepo = new Mock<IGiftRepository>();
            mockGiftRepo.Setup(r => r.GetGiftsAsync()).ReturnsAsync(new List<Gift> { gift });

            var users = new List<User?> { null };
            var mockLotteryRepo = new Mock<ILotteryRepository>();
            mockLotteryRepo.Setup(r => r.LotteryAsync(gift)).ReturnsAsync(users.Cast<User>().ToList());

            var service = new LotteryService(mockLotteryRepo.Object, mockGiftRepo.Object, new Mock<ILogger<LotteryService>>().Object);

            // If users list contains null, selection may be null -> method returns false
            var result = await service.LotteryAsync();

            Assert.False(result);
        }

        [Fact]
        public async Task LotteryAsync_CreateLotteryReturnsNull_ThrowsException()
        {
            var gift = new Gift { Id = 4, Name = "g4" };
            var user = new User { Id = 10 };
            var mockGiftRepo = new Mock<IGiftRepository>();
            mockGiftRepo.Setup(r => r.GetGiftsAsync()).ReturnsAsync(new List<Gift> { gift });

            var mockLotteryRepo = new Mock<ILotteryRepository>();
            mockLotteryRepo.Setup(r => r.LotteryAsync(gift)).ReturnsAsync(new List<User> { user });
            mockLotteryRepo.Setup(r => r.CreateLottery(gift.Id, user.Id)).ThrowsAsync(new Exception("create failed"));

            var service = new LotteryService(mockLotteryRepo.Object, mockGiftRepo.Object, new Mock<ILogger<LotteryService>>().Object);

            await Assert.ThrowsAsync<Exception>(() => service.LotteryAsync());
        }

        [Fact]
        public async Task SendingEmail_NullInputs_ThrowsArgumentNullException()
        {
            var service = new LotteryService(new Mock<ILotteryRepository>().Object, new Mock<IGiftRepository>().Object, new Mock<ILogger<LotteryService>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.SendingEmail(null!, null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.SendingEmail(new User(), null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.SendingEmail(null!, new Gift()));
        }
    }
}
