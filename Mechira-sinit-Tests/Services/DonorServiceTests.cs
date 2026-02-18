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
    public class DonorServiceTests
    {
        [Fact]
        public async Task CreateDonorAsync_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.CreateDonorAsync(It.IsAny<Donor>())).ReturnsAsync(true);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var dto = new DonorCreateDto { Id = 0, FirstName = "A", LastName = "B", Phone = "p", Email = "e@x.com" };

            var result = await service.CreateDonorAsync(dto);

            Assert.True(result);
            mockRepo.Verify(r => r.CreateDonorAsync(It.Is<Donor>(d => d.FirstName == dto.FirstName && d.Email == dto.Email)), Times.Once);
        }

        [Fact]
        public async Task CreateDonorAsync_Null_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateDonorAsync(null!));
        }

        [Fact]
        public async Task CreateDonorAsync_CreateFails_ThrowsException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.CreateDonorAsync(It.IsAny<Donor>())).ReturnsAsync(false);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var dto = new DonorCreateDto { Id = 0, FirstName = "A", LastName = "B", Phone = "p", Email = "e@x.com" };

            await Assert.ThrowsAsync<Exception>(() => service.CreateDonorAsync(dto));
        }

        [Fact]
        public async Task DeleteDonorAsync_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.DeleteDonorAsync(1)).ReturnsAsync(true);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var result = await service.DeleteDonorAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteDonorAsync_InvalidId_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteDonorAsync(0));
        }

        [Fact]
        public async Task DeleteDonorAsync_NotFound_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.DeleteDonorAsync(2)).ReturnsAsync(false);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteDonorAsync(2));
        }

        [Fact]
        public async Task GetAllDonors_ReturnsMappedList()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var donors = new List<Donor>
            {
                new Donor { Id = 1, FirstName = "John", LastName = "Doe", Email = "j@e.com", Phone = "111" },
                new Donor { Id = 2, FirstName = "Jane", LastName = "Roe", Email = "jane@e.com", Phone = "222" }
            };
            mockRepo.Setup(r => r.GetAllDonorsAsync()).ReturnsAsync(donors);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var result = await service.GetAllDonors();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, d => d.Email == "j@e.com");
        }

        [Fact]
        public async Task GetAllDonors_NullRepositoryResult_Throws()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.GetAllDonorsAsync()).ReturnsAsync((List<Donor>?)null);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetAllDonors());
        }

        [Fact]
        public async Task GetById_Success_ReturnsDonor()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var donor = new Donor { Id = 5, FirstName = "F", LastName = "L" };
            mockRepo.Setup(r => r.GetById(5)).ReturnsAsync(donor);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var result = await service.GetById(5);

            Assert.Equal(donor, result);
        }

        [Fact]
        public async Task GetById_NotFound_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.GetById(6)).ReturnsAsync((Donor?)null);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetById(6));
        }

        [Fact]
        public async Task UpdateDonorAsync_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var existing = new Donor { Id = 3, FirstName = "Old", LastName = "Name", Email = "o@e.com", Phone = "p" };
            mockRepo.Setup(r => r.GetById(3)).ReturnsAsync(existing);
            mockRepo.Setup(r => r.UpdateDonorAsync(It.IsAny<Donor>())).ReturnsAsync(true);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var dto = new DtoDonors { FirstName = "New", LastName = "Name", Email = "n@e.com", Phone = "pp" };

            var result = await service.UpdateDonorAsync(dto, 3);

            Assert.True(result);
            mockRepo.Verify(r => r.UpdateDonorAsync(It.Is<Donor>(d => d.FirstName == "New" && d.Email == "n@e.com")), Times.Once);
        }

        [Fact]
        public async Task UpdateDonorAsync_NullDonor_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateDonorAsync(null!, 1));
        }

        [Fact]
        public async Task UpdateDonorAsync_InvalidId_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var dto = new DtoDonors { FirstName = "a" };
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateDonorAsync(dto, 0));
        }

        [Fact]
        public async Task UpdateDonorAsync_NotFound_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.GetById(7)).ReturnsAsync((Donor?)null);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var dto = new DtoDonors { FirstName = "a" };
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateDonorAsync(dto, 7));
        }

        [Fact]
        public async Task UpdateDonorAsync_UpdateFails_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var existing = new Donor { Id = 8, FirstName = "X" };
            mockRepo.Setup(r => r.GetById(8)).ReturnsAsync(existing);
            mockRepo.Setup(r => r.UpdateDonorAsync(It.IsAny<Donor>())).ReturnsAsync(false);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var dto = new DtoDonors { FirstName = "Y" };
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateDonorAsync(dto, 8));
        }

        [Fact]
        public async Task GetByName_Success_ReturnsDonorCreateDto()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var donor = new Donor { Id = 9, FirstName = "N", LastName = "L", Email = "a@b.com", Phone = "p" };
            mockRepo.Setup(r => r.GetByName("N")).ReturnsAsync(donor);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var result = await service.GetByName("N");

            Assert.Equal(donor.Id, result.Id);
            Assert.Equal(donor.FirstName, result.FirstName);
        }

        [Fact]
        public async Task GetByName_NotFound_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.GetByName("x")).ReturnsAsync((Donor?)null);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByName("x"));
        }

        [Fact]
        public async Task GetByEmail_NullOrEmpty_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GetByEmail(string.Empty));
        }

        [Fact]
        public async Task GetByEmail_NotFound_ThrowsKeyNotFoundException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.GetByEmail("no@e.com")).ReturnsAsync((Donor?)null);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByEmail("no@e.com"));
        }

        [Fact]
        public async Task AddDonation_Success_ReturnsTrue()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.AddDonation(1, 2)).ReturnsAsync(true);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            var result = await service.AddDonation(1, 2);

            Assert.True(result);
        }

        [Fact]
        public async Task AddDonation_InvalidIds_ThrowsArgumentException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddDonation(0, 1));
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddDonation(1, 0));
        }

        [Fact]
        public async Task AddDonation_RepoFails_ThrowsException()
        {
            var mockRepo = new Mock<IDonorRepository>();
            mockRepo.Setup(r => r.AddDonation(3, 4)).ReturnsAsync(false);

            var service = new DonorServise(mockRepo.Object, new Mock<ILogger<DonorServise>>().Object);

            await Assert.ThrowsAsync<Exception>(() => service.AddDonation(3, 4));
        }
    }
}
