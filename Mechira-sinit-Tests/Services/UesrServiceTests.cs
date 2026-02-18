using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Tests.Services
{
    public class UesrServiceTests
    {
        [Fact]
        public async Task GetUserByIdAsync_Success_ReturnsDto()
        {
            var mockRepo = new Mock<IUsersRepository>();
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("pwd")),
                Phone = "123",
                Address = "addr"
            };
            mockRepo.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

            var service = new UesrService(mockRepo.Object, new Mock<ILogger<UesrService>>().Object, new Mock<ITokenService>().Object, new Mock<IConfiguration>().Object);

            var result = await service.GetUserByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.FirstName, result.FirstName);
        }

        [Fact]
        public async Task GetUserByIdAsync_NotFound_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IUsersRepository>();
            mockRepo.Setup(r => r.GetUserByIdAsync(2)).ReturnsAsync((User?)null);

            var service = new UesrService(mockRepo.Object, new Mock<ILogger<UesrService>>().Object, new Mock<ITokenService>().Object, new Mock<IConfiguration>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetUserByIdAsync(2));
        }

        [Fact]
        public async Task CreateUserAsync_Success_ReturnsDto()
        {
            var mockRepo = new Mock<IUsersRepository>();
            mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());
            mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

            var service = new UesrService(mockRepo.Object, new Mock<ILogger<UesrService>>().Object, new Mock<ITokenService>().Object, new Mock<IConfiguration>().Object);

            var input = new User { Id = 0, FirstName = "A", LastName = "B", Email = "a@b.com", Password = "secret", Phone = "p", Address = "addr" };

            var result = await service.CreateUserAsync(input);

            Assert.NotNull(result);
            Assert.Equal(input.Email, result.Email);
        }

        [Fact]
        public async Task CreateUserAsync_NullUser_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IUsersRepository>();
            var service = new UesrService(mockRepo.Object, new Mock<ILogger<UesrService>>().Object, new Mock<ITokenService>().Object, new Mock<IConfiguration>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateUserAsync(null!));
        }

        [Fact]
        public async Task CreateUserAsync_CreateReturnsNull_ThrowsException()
        {
            var mockRepo = new Mock<IUsersRepository>();
            mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());
            mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync((User?)null);

            var service = new UesrService(mockRepo.Object, new Mock<ILogger<UesrService>>().Object, new Mock<ITokenService>().Object, new Mock<IConfiguration>().Object);

            var input = new User { Id = 0, FirstName = "A", LastName = "B", Email = "a@b.com", Password = "secret", Phone = "p", Address = "addr" };

            await Assert.ThrowsAsync<Exception>(() => service.CreateUserAsync(input));
        }
    }
}
