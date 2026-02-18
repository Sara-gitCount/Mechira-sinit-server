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
    public class DonorsControllerTests
    {
        [Fact]
        public async Task GetAllDonors_ServiceReturnsList_ReturnsOk()
        {
            var mockService = new Mock<IDonorServise>();
            var list = new List<DtoDonors> { new DtoDonors { Email = "a@b" } };
            mockService.Setup(s => s.GetAllDonors()).ReturnsAsync(list);

            var controller = new DonorsController(mockService.Object, new Mock<ILogger<DonorsController>>().Object);

            var action = await controller.GetAllDonors();

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<List<DtoDonors>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetAllDonors_ServiceThrows_Returns500()
        {
            var mockService = new Mock<IDonorServise>();
            mockService.Setup(s => s.GetAllDonors()).ThrowsAsync(new Exception("boom"));

            var controller = new DonorsController(mockService.Object, new Mock<ILogger<DonorsController>>().Object);

            var action = await controller.GetAllDonors();

            var obj = Assert.IsType<ObjectResult>(action.Result);
            Assert.Equal(500, obj.StatusCode);
            Assert.Contains("boom", obj.Value?.ToString());
        }

        [Fact]
        public async Task GetByName_ServiceReturns_ReturnsOk()
        {
            var mockService = new Mock<IDonorServise>();
            var dto = new DonorCreateDto { Id = 1, FirstName = "A" };
            mockService.Setup(s => s.GetByName("A")).ReturnsAsync(dto);

            var controller = new DonorsController(mockService.Object, new Mock<ILogger<DonorsController>>().Object);

            var action = await controller.GetByName("A");

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var value = Assert.IsType<DonorCreateDto>(ok.Value);
            Assert.Equal(1, value.Id);
        }

        [Fact]
        public async Task CreateDonor_ServiceSucceeds_ReturnsOk()
        {
            var mockService = new Mock<IDonorServise>();
            mockService.Setup(s => s.CreateDonorAsync(It.IsAny<DonorCreateDto>())).ReturnsAsync(true);

            var controller = new DonorsController(mockService.Object, new Mock<ILogger<DonorsController>>().Object);

            var result = await controller.createDonor(new DonorCreateDto { FirstName = "A" });

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task CreateDonor_ServiceThrowsArgumentNull_ReturnsBadRequest()
        {
            var mockService = new Mock<IDonorServise>();
            mockService.Setup(s => s.CreateDonorAsync(It.IsAny<DonorCreateDto>())).ThrowsAsync(new ArgumentNullException("d"));

            var controller = new DonorsController(mockService.Object, new Mock<ILogger<DonorsController>>().Object);

            var result = await controller.createDonor(new DonorCreateDto { FirstName = "A" });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Bad request", bad.Value?.ToString());
        }

        [Fact]
        public async Task CreateDonor_ServiceThrowsGeneric_Returns500()
        {
            var mockService = new Mock<IDonorServise>();
            mockService.Setup(s => s.CreateDonorAsync(It.IsAny<DonorCreateDto>())).ThrowsAsync(new Exception("fail"));

            var controller = new DonorsController(mockService.Object, new Mock<ILogger<DonorsController>>().Object);

            var result = await controller.createDonor(new DonorCreateDto { FirstName = "A" });

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
            Assert.Contains("fail", obj.Value?.ToString());
        }

        [Fact]
        public async Task UpdateDonor_ServiceThrowsKeyNotFound_ReturnsNotFound()
        {
            var mockService = new Mock<IDonorServise>();
            mockService.Setup(s => s.UpdateDonorAsync(It.IsAny<DtoDonors>(), 5)).ThrowsAsync(new KeyNotFoundException("nf"));

            var controller = new DonorsController(mockService.Object, new Mock<ILogger<DonorsController>>().Object);

            var result = await controller.UpdateDonor(new DtoDonors { FirstName = "A" }, 5);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Not found", notFound.Value?.ToString());
        }

        [Fact]
        public async Task AddDonation_ServiceReturnsTrue_ReturnsOkBool()
        {
            var mockService = new Mock<IDonorServise>();
            mockService.Setup(s => s.AddDonation(1, 2)).ReturnsAsync(true);

            var controller = new DonorsController(mockService.Object, new Mock<ILogger<DonorsController>>().Object);

            var action = await controller.AddDonation(1, 2);

            var ok = Assert.IsType<OkObjectResult>(action.Result);
            Assert.True((bool)ok.Value);
        }

        [Fact]
        public async Task AddDonation_ServiceThrowsArgument_ReturnsBadRequest()
        {
            var mockService = new Mock<IDonorServise>();
            mockService.Setup(s => s.AddDonation(0, 2)).ThrowsAsync(new ArgumentException("id"));

            var controller = new DonorsController(mockService.Object, new Mock<ILogger<DonorsController>>().Object);

            var action = await controller.AddDonation(0, 2);

            var bad = Assert.IsType<BadRequestObjectResult>(action.Result);
            Assert.Contains("Bad request", bad.Value?.ToString());
        }
    }
}
