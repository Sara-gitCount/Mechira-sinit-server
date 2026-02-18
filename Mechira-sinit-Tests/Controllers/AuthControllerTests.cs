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
            var controller = CreateController();
            var dto = new LoginRequestDto { Email = "", Password = "" };

            var result = await controller.Login(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenAuthenticationFails()
        {
            var controller = CreateController();
            var dto = new LoginRequestDto { Email = "user@test.com", Password = "wrong" };
            mockUsersService.Setup(s => s.AuthenticateAsync(dto.Email, dto.Password))
                            .ReturnsAsync((LoginResponseDto?)null);

            var result = await controller.Login(dto);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenAuthenticationSucceeds()
        {
            var controller = CreateController();
            var dto = new LoginRequestDto { Email = "user@test.com", Password = "correct" };
            var response = new LoginResponseDto { Token = "tok", User = new User { Email = dto.Email } };
            mockUsersService.Setup(s => s.AuthenticateAsync(dto.Email, dto.Password))
                            .ReturnsAsync(response);

            var result = await controller.Login(dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task Register_ReturnsCreated_WhenUserCreated()
        {
            var controller = CreateController();
            var user = new User { Id = 123, Email = "new@user.com", Password = "pw" };
            var dto = new WebApplication1.Dto.DtoUser { FirstName = "F", LastName = "L", Email = user.Email, Phone = "123" };
            mockUsersService.Setup(s => s.CreateUserAsync(user)).ReturnsAsync(dto);

            var result = await controller.Register(user);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(user, created.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenServiceThrowsArgumentException()
        {
            var controller = CreateController();
            var user = new User { Id = 0, Email = "bad@user.com", Password = "pw" };
            mockUsersService.Setup(s => s.CreateUserAsync(user))
                            .ThrowsAsync(new ArgumentException("invalid"));

            var result = await controller.Register(user);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
