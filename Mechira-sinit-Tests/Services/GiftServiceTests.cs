using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GiftServiceTests
    {
        [Fact]
        public async Task AddGift_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IGiftRepository>();
            mockRepo.Setup(r => r.AddGift(It.IsAny<Gift>())).ReturnsAsync(true);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var dto = new DtoGifts { Name = "G1", Description = "d", CategoryId = 1, DonorId = 2, Price = 10, Image = "i" };

            var result = await service.AddGift(dto);

            Assert.True(result);
            mockRepo.Verify(r => r.AddGift(It.Is<Gift>(g => g.Name == dto.Name && g.Price == dto.Price)), Times.Once);
        }

        [Fact]
        public async Task AddGift_Null_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddGift(null!));
        }

        [Fact]
        public async Task AddGift_RepoFails_ThrowsException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            mockRepo.Setup(r => r.AddGift(It.IsAny<Gift>())).ReturnsAsync(false);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var dto = new DtoGifts { Name = "G1" };

            await Assert.ThrowsAsync<Exception>(() => service.AddGift(dto));
        }

        [Fact]
        public async Task DeleteGiftAsync_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IGiftRepository>();
            mockRepo.Setup(r => r.DeleteGiftAsync(5)).ReturnsAsync(true);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var result = await service.DeleteGiftAsync(5);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteGiftAsync_InvalidId_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteGiftAsync(0));
        }

        [Fact]
        public async Task DeleteGiftAsync_NotFound_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            mockRepo.Setup(r => r.DeleteGiftAsync(6)).ReturnsAsync(false);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteGiftAsync(6));
        }

        [Fact]
        public async Task GetDonorsAsync_Success_ReturnsDtoDonors()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var donor = new Donor { Id = 1, FirstName = "A", LastName = "B", Email = "e@x" };
            mockRepo.Setup(r => r.GetDonorsAsync(1)).ReturnsAsync(donor);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var result = await service.GetDonorsAsync(1);

            Assert.Equal(donor.FirstName, result.FirstName);
        }

        [Fact]
        public async Task GetDonorsAsync_InvalidId_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GetDonorsAsync(0));
        }

        [Fact]
        public async Task GetDonorsAsync_NotFound_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            mockRepo.Setup(r => r.GetDonorsAsync(2)).ReturnsAsync((Donor?)null);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetDonorsAsync(2));
        }

        [Fact]
        public async Task GetGiftByDonor_Success_ReturnsList()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var gifts = new List<Gift> { new Gift { Id = 1, Name = "g", Description = "d", CategoryId = 1 } };
            mockRepo.Setup(r => r.GetGiftByDonor("donor")).ReturnsAsync(gifts);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var result = await service.GetGiftByDonor("donor");

            Assert.Single(result);
            Assert.Equal("g", result.First().Name);
        }

        [Fact]
        public async Task GetGiftByDonor_NullName_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetGiftByDonor(null!));
        }

        [Fact]
        public async Task GetGiftByDonor_RepoReturnsEmpty_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            mockRepo.Setup(r => r.GetGiftByDonor("d")).ReturnsAsync(new List<Gift>());

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetGiftByDonor("d"));
        }

        [Fact]
        public async Task GetGiftByName_Success_ReturnsDto()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var gift = new Gift { Id = 3, Name = "X", Description = "d", Image = "i", Price = 5, Category = new Category { Name = "c" } };
            mockRepo.Setup(r => r.GetGiftByName("X")).ReturnsAsync(gift);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var result = await service.GetGiftByName("X");

            Assert.Equal("X", result.Name);
            Assert.Equal("c", result.CategoryName);
        }

        [Fact]
        public async Task GetGiftByName_Null_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetGiftByName(null!));
        }

        [Fact]
        public async Task GetGiftByNOfUsers_Negative_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GetGiftByNOfUsers(-1));
        }

        [Fact]
        public async Task GetGiftsAsync_RepoReturnsEmpty_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            mockRepo.Setup(r => r.GetGiftsAsync()).ReturnsAsync(new List<Gift>());

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetGiftsAsync());
        }

        [Fact]
        public async Task UpdateGiftAsync_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var existing = new Gift { Id = 10, Name = "old" };
            mockRepo.Setup(r => r.GetById(10)).ReturnsAsync(existing);
            mockRepo.Setup(r => r.UpdateGiftAsync(It.IsAny<Gift>())).ReturnsAsync(true);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var dto = new DtoGifts { Name = "new", Description = "d", CategoryId = 2, DonorId = 3, Price = 9, Image = "i" };

            var result = await service.UpdateGiftAsync(dto, 10);

            Assert.True(result);
            mockRepo.Verify(r => r.UpdateGiftAsync(It.Is<Gift>(g => g.Name == "new" && g.Id == 10)), Times.Once);
        }

        [Fact]
        public async Task UpdateGiftAsync_NullGift_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateGiftAsync(null!, 1));
        }

        [Fact]
        public async Task UpdateGiftAsync_InvalidId_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var dto = new DtoGifts { Name = "n" };
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateGiftAsync(dto, 0));
        }

        [Fact]
        public async Task UpdateGiftAsync_NotFound_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            mockRepo.Setup(r => r.GetById(11)).ReturnsAsync((Gift?)null);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var dto = new DtoGifts { Name = "n" };
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateGiftAsync(dto, 11));
        }

        [Fact]
        public async Task GetGiftById_Success_ReturnsDto()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var gift = new Gift { Id = 20, Name = "G", CategoryId = 2, DonorId = 3, Price = 4, Category = new Category { Name = "Cat" } };
            mockRepo.Setup(r => r.GetById(20)).ReturnsAsync(gift);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var result = await service.GetGiftById(20);

            Assert.Equal("G", result.Name);
            Assert.Equal(2, result.CategoryId);
        }

        [Fact]
        public async Task GetGiftById_Zero_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetGiftById(0));
        }

        [Fact]
        public async Task GetOrderByPrice_Category_ReturnsMappedList()
        {
            var mockRepo = new Mock<IGiftRepository>();
            var gifts = new List<Gift> { new Gift { Name = "x", Description = "d", Image = "i", Price = 1, DonorId = 2, Category = new Category { Name = "c" } } };
            mockRepo.Setup(r => r.GetOrderByPrice_Category()).ReturnsAsync(gifts);

            var service = new GiftService(mockRepo.Object, new Mock<ILogger<GiftService>>().Object);

            var result = await service.GetOrderByPrice_Category();

            Assert.Single(result);
            Assert.Equal("c", result.First().CategoryName);
        }
    }
}
