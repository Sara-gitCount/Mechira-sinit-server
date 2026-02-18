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
    public class OrderServiceTests
    {
        [Fact]
        public async Task GetAllUsers_RepoReturnsUsers_ReturnsMappedList()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            var users = new List<User>
            {
                new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "j@e.com", Phone = "123" }
            };
            mockRepo.Setup(r => r.GetAllUsers()).ReturnsAsync(users);

            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            var result = await service.GetAllUsers();

            Assert.Single(result);
            Assert.Equal("John", result.First().FirstName);
        }

        [Fact]
        public async Task GetAllUsers_EmptyRepo_ReturnsEmptyList()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            mockRepo.Setup(r => r.GetAllUsers()).ReturnsAsync(new List<User>());

            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            var result = await service.GetAllUsers();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGiftOrderByOrders_MappedSuccessfully()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            var gift = new Gift { Id = 1, Name = "G" };
            var group = new List<Order> { new Order { User = new User { FirstName = "A", LastName = "B" }, Gift = gift } };
            var groups = new List<IGrouping<Gift, Order>> { new GroupingStub<Gift, Order>(gift, group) };
            mockRepo.Setup(r => r.GetOrdersByGifts()).ReturnsAsync(groups);

            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            var result = await service.GetOrdersByGifts();

            Assert.Single(result);
            Assert.Equal("G", result.First().GiftName);
            Assert.Contains("A B", result.First().Users);
        }

        [Fact]
        public async Task CreateOrder_InvalidIds_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrder(0, 1));
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrder(1, 0));
        }

        [Fact]
        public async Task CreateOrder_RepoReturnsFalse_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            mockRepo.Setup(r => r.CreateOrder(1, 2)).ReturnsAsync(false);
            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateOrder(1, 2));
        }

        [Fact]
        public async Task CreateOrder_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            mockRepo.Setup(r => r.CreateOrder(1, 2)).ReturnsAsync(true);
            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            var result = await service.CreateOrder(1, 2);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteOrder_InvalidId_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteOrder(0));
        }

        [Fact]
        public async Task DeleteOrder_RepoReturnsFalse_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            mockRepo.Setup(r => r.DeleteOrder(5)).ReturnsAsync(false);
            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteOrder(5));
        }

        [Fact]
        public async Task DeleteOrder_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            mockRepo.Setup(r => r.DeleteOrder(6)).ReturnsAsync(true);
            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            var result = await service.DeleteOrder(6);

            Assert.True(result);
        }

        [Fact]
        public async Task ChangeStatus_InvalidId_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.ChangeStatus(0));
        }

        [Fact]
        public async Task ChangeStatus_RepoReturnsFalse_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            mockRepo.Setup(r => r.ChangeStatus(7)).ReturnsAsync(false);
            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ChangeStatus(7));
        }

        [Fact]
        public async Task ChangeStatus_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IOrdersRepository>();
            mockRepo.Setup(r => r.ChangeStatus(8)).ReturnsAsync(true);
            var service = new OrderService(mockRepo.Object, new Mock<ILogger<OrderService>>().Object);

            var result = await service.ChangeStatus(8);

            Assert.True(result);
        }
    }

    // Simple grouping stub to simulate IGrouping<Gift, Order>
    internal class GroupingStub<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> _items;
        public GroupingStub(TKey key, IEnumerable<TElement> items)
        {
            Key = key;
            _items = items;
        }

        public TKey Key { get; }
        public IEnumerator<TElement> GetEnumerator() => _items.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
