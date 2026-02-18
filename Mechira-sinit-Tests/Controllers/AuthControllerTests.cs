using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using Xunit;
using static WebApplication1.Dto.DtoAuth;

namespace WebApplication1.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUsersService> mockUsersService = new();
        private readonly Mock<ILogger<AuthController>> mockLogger = new();

        private AuthController CreateController()
        {
            return new AuthController(mockUsersService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenEmailOrPasswordMissing()
        {
            // Arrange
            var controller = CreateController();
            var dto = new LoginRequestDto { Email = "", Password = "" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenAuthenticationFails()
        {
            // Arrange
            var controller = CreateController();
            var dto = new LoginRequestDto { Email = "user@test.com", Password = "wrong" };
            mockUsersService.Setup(s => s.AuthenticateAsync(dto.Email, dto.Password))
                            .ReturnsAsync((LoginResponseDto?)null);

            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenAuthenticationSucceeds()
        {
            // Arrange
            var controller = CreateController();
            var dto = new LoginRequestDto { Email = "user@test.com", Password = "correct" };
            var response = new LoginResponseDto { Token = "tok", User = new User { Email = dto.Email } };
            mockUsersService.Setup(s => s.AuthenticateAsync(dto.Email, dto.Password))
                            .ReturnsAsync(response);

            // Act
            var result = await controller.Login(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task Register_ReturnsCreated_WhenUserCreated()
        {
            // Arrange
            var controller = CreateController();
            var user = new User { Id = 123, Email = "new@user.com", Password = "pw" };
            var dto = new WebApplication1.Dto.DtoUser { FirstName = "F", LastName = "L", Email = user.Email, Phone = "123" };
            mockUsersService.Setup(s => s.CreateUserAsync(user)).ReturnsAsync(dto);

            // Act
            var result = await controller.Register(user);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(user, created.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenServiceThrowsArgumentException()
        {
            // Arrange
            var controller = CreateController();
            var user = new User { Id = 0, Email = "bad@user.com", Password = "pw" };
            mockUsersService.Setup(s => s.CreateUserAsync(user))
                            .ThrowsAsync(new ArgumentException("invalid"));

            // Act
            var result = await controller.Register(user);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
