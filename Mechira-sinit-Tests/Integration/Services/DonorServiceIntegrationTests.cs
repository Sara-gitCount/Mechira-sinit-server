using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Data;
using WebApplication1.Dto;
using WebApplication1.Models;
using WebApplication1.Repository;
using WebApplication1.Services;
using WebApplication1.Tests.Helpers;
using Xunit;

namespace WebApplication1.Tests.Integration.Services
{
    /// <summary>
    /// Integration tests for DonorService.
    /// Tests the full flow from service to repository with InMemory database.
    /// Each test uses an isolated InMemory context to ensure proper cleanup.
    /// </summary>
    public class DonorServiceIntegrationTests
    {
        private readonly Mock<ILogger<DonorServise>> _mockLogger = new();

        private (StoreContext context, DonorRepository repository, DonorServise service) CreateServiceWithContext()
        {
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var repository = new DonorRepository(context);
            var service = new DonorServise(repository, _mockLogger.Object);
            return (context, repository, service);
        }

        #region CreateDonorAsync Tests

        [Fact]
        public async Task CreateDonorAsync_WithValidData_ShouldCreateDonorSuccessfully()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var donor = new DonorCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Phone = "1234567890"
            };

            // Act
            var result = await service.CreateDonorAsync(donor);

            // Assert
            Assert.True(result);
            var createdDonor = context.donors.FirstOrDefault(d => d.Email == donor.Email);
            Assert.NotNull(createdDonor);
            Assert.Equal("John", createdDonor.FirstName);
            context.Dispose();
        }

        [Fact]
        public async Task CreateDonorAsync_WithNullDonor_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateDonorAsync(null!));
            context.Dispose();
        }

        #endregion

        #region DeleteDonorAsync Tests

        [Fact]
        public async Task DeleteDonorAsync_WithValidId_ShouldDeleteDonorSuccessfully()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            
            var donor = new Donor
            {
                Id = 1,
                FirstName = "Tom",
                LastName = "Jerry",
                Email = "tom@example.com",
                Phone = "1111111111",
                Donations = new List<Gift>()
            };
            context.donors.Add(donor);
            context.SaveChanges();

            // Act
            var result = await service.DeleteDonorAsync(1);

            // Assert
            Assert.True(result);
            var deletedDonor = context.donors.FirstOrDefault(d => d.Id == 1);
            Assert.Null(deletedDonor);
            context.Dispose();
        }

        [Fact]
        public async Task DeleteDonorAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteDonorAsync(9999));
            context.Dispose();
        }

        [Fact]
        public async Task DeleteDonorAsync_WithZeroId_ShouldThrowArgumentException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteDonorAsync(0));
            context.Dispose();
        }

        #endregion

        #region GetAllDonors Tests

        [Fact]
        public async Task GetAllDonors_WithMultipleDonors_ShouldReturnAllDonors()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var donors = new List<Donor>
            {
                new Donor { Id = 1, FirstName = "Alice", LastName = "A", Email = "alice@example.com", Phone = "111", Donations = new List<Gift>() },
                new Donor { Id = 2, FirstName = "Bob", LastName = "B", Email = "bob@example.com", Phone = "222", Donations = new List<Gift>() },
                new Donor { Id = 3, FirstName = "Charlie", LastName = "C", Email = "charlie@example.com", Phone = "333", Donations = new List<Gift>() }
            };
            
            foreach (var donor in donors)
            {
                context.donors.Add(donor);
            }
            context.SaveChanges();

            // Act
            var results = await service.GetAllDonors();

            // Assert
            Assert.Equal(3, results.Count);
            Assert.Contains("Alice", results.Select(d => d.FirstName));
            context.Dispose();
        }

        [Fact]
        public async Task GetAllDonors_WithNoDonors_ShouldReturnEmptyList()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act
            var results = await service.GetAllDonors();

            // Assert
            Assert.Empty(results);
            context.Dispose();
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnDonor()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var donor = new Donor
            {
                Id = 5,
                FirstName = "David",
                LastName = "D",
                Email = "david@example.com",
                Phone = "4444444444",
                Donations = new List<Gift>()
            };
            context.donors.Add(donor);
            context.SaveChanges();

            // Act
            var result = await service.GetById(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("David", result.FirstName);
            context.Dispose();
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetById(9999));
            context.Dispose();
        }

        #endregion

        #region UpdateDonorAsync Tests

        [Fact]
        public async Task UpdateDonorAsync_WithValidData_ShouldUpdateDonorSuccessfully()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var donor = new Donor
            {
                Id = 10,
                FirstName = "Eve",
                LastName = "E",
                Email = "eve@example.com",
                Phone = "5555555555",
                Donations = new List<Gift>()
            };
            context.donors.Add(donor);
            context.SaveChanges();

            var updateDto = new DtoDonors
            {
                FirstName = "Eva",
                LastName = "Updated",
                Email = "eva.updated@example.com",
                Phone = "5555555556"
            };

            // Act
            var result = await service.UpdateDonorAsync(updateDto, 10);

            // Assert
            Assert.True(result);
            var updatedDonor = context.donors.FirstOrDefault(d => d.Id == 10);
            Assert.NotNull(updatedDonor);
            Assert.Equal("Eva", updatedDonor.FirstName);
            context.Dispose();
        }

        [Fact]
        public async Task UpdateDonorAsync_WithNullDto_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateDonorAsync(null!, 1));
            context.Dispose();
        }

        [Fact]
        public async Task UpdateDonorAsync_WithZeroId_ShouldThrowArgumentException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var updateDto = new DtoDonors
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "test@example.com",
                Phone = "1111111111"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateDonorAsync(updateDto, 0));
            context.Dispose();
        }

        [Fact]
        public async Task UpdateDonorAsync_WithNonExistentId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var updateDto = new DtoDonors
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "test@example.com",
                Phone = "1111111111"
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateDonorAsync(updateDto, 9999));
            context.Dispose();
        }

        #endregion

        #region GetByEmail Tests

        [Fact]
        public async Task GetByEmail_WithValidEmail_ShouldReturnDonor()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var donor = new Donor
            {
                Id = 15,
                FirstName = "Frank",
                LastName = "F",
                Email = "frank@example.com",
                Phone = "6666666666",
                Donations = new List<Gift>()
            };
            context.donors.Add(donor);
            context.SaveChanges();

            // Act
            var result = await service.GetByEmail("frank@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Frank", result.FirstName);
            context.Dispose();
        }

        [Fact]
        public async Task GetByEmail_WithNullEmail_ShouldThrowArgumentException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetByEmail(null!));
            context.Dispose();
        }

        [Fact]
        public async Task GetByEmail_WithNonExistentEmail_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByEmail("nonexistent@example.com"));
            context.Dispose();
        }

        #endregion

        #region GetByName Tests

        [Fact]
        public async Task GetByName_WithValidName_ShouldReturnDonor()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var donor = new Donor
            {
                Id = 20,
                FirstName = "Grace",
                LastName = "G",
                Email = "grace@example.com",
                Phone = "7777777777",
                Donations = new List<Gift>()
            };
            context.donors.Add(donor);
            context.SaveChanges();

            // Act - GetByName searches for "FirstName LastName" format
            var result = await service.GetByName("Grace G");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Grace", result.FirstName);
            context.Dispose();
        }

        [Fact]
        public async Task GetByName_WithNonExistentName_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByName("NonExistent Unknown"));
            context.Dispose();
        }

        #endregion

        #region AddDonation Tests

        [Fact]
        public async Task AddDonation_WithValidIds_ShouldAddDonationSuccessfully()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            
            var donor = new Donor
            {
                Id = 25,
                FirstName = "Henry",
                LastName = "H",
                Email = "henry@example.com",
                Phone = "8888888888",
                Donations = new List<Gift>()
            };
            context.donors.Add(donor);

            var gift = new Gift
            {
                Id = 100,
                Name = "Test Gift",
                Description = "A test gift",
                Image = "image.jpg",
                Price = 50,
                CategoryId = 1,
                DonorId = 25,
                Donor = donor,
                Orders = new List<Order>()
            };
            context.gifts.Add(gift);
            context.SaveChanges();

            // Act
            var result = await service.AddDonation(25, 100);

            // Assert
            Assert.True(result);
            context.Dispose();
        }

        [Fact]
        public async Task AddDonation_WithInvalidDonorId_ShouldThrowArgumentException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddDonation(0, 1));
            context.Dispose();
        }

        [Fact]
        public async Task AddDonation_WithInvalidGiftId_ShouldThrowArgumentException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddDonation(1, 0));
            context.Dispose();
        }

        #endregion
    }
}
