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
    /// Integration tests for GiftService.
    /// Tests the full flow from service to repository with InMemory database.
    /// Each test uses an isolated InMemory context to ensure proper cleanup.
    /// </summary>
    public class GiftServiceIntegrationTests
    {
        private readonly Mock<ILogger<GiftService>> _mockLogger = new();

        private (StoreContext context, GiftRepository repository, GiftService service) CreateServiceWithContext()
        {
            var context = InMemoryDbContextFactory.CreateInMemoryContext();
            var repository = new GiftRepository(context);
            var service = new GiftService(repository, _mockLogger.Object);
            return (context, repository, service);
        }

        #region AddGift Tests

        [Fact]
        public async Task AddGift_WithValidData_ShouldAddGiftSuccessfully()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            
            // Create category first
            var category = new Category { Id = 1, Name = "Books" };
            context.categories.Add(category);
            
            // Create donor
            var donor = new Donor
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Donations = new List<Gift>()
            };
            context.donors.Add(donor);
            context.SaveChanges();

            var giftDto = new DtoGifts
            {
                Name = "Test Book",
                Description = "A great book",
                CategoryId = 1,
                DonorId = 1,
                Price = 25,
                Image = "book.jpg"
            };

            // Act
            var result = await service.AddGift(giftDto);

            // Assert
            Assert.True(result);
            var addedGift = context.gifts.FirstOrDefault(g => g.Name == "Test Book");
            Assert.NotNull(addedGift);
            context.Dispose();
        }

        [Fact]
        public async Task AddGift_WithNullGift_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddGift(null!));
            context.Dispose();
        }

        #endregion

        #region DeleteGiftAsync Tests

        [Fact]
        public async Task DeleteGiftAsync_WithValidId_ShouldDeleteGiftSuccessfully()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            var gift = new Gift
            {
                Id = 1,
                Name = "To Delete",
                Description = "Will be deleted",
                Image = "delete.jpg",
                Price = 15,
                CategoryId = 1,
                DonorId = 1,
                Orders = new List<Order>()
            };
            context.gifts.Add(gift);
            context.SaveChanges();

            // Act
            var result = await service.DeleteGiftAsync(1);

            // Assert
            Assert.True(result);
            var deletedGift = context.gifts.FirstOrDefault(g => g.Id == 1);
            Assert.Null(deletedGift);
            context.Dispose();
        }

        [Fact]
        public async Task DeleteGiftAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteGiftAsync(9999));
            context.Dispose();
        }

        [Fact]
        public async Task DeleteGiftAsync_WithZeroId_ShouldThrowArgumentException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteGiftAsync(0));
            context.Dispose();
        }

        #endregion

        #region GetGiftsAsync Tests

        [Fact]
        public async Task GetGiftsAsync_WithMultipleGifts_ShouldReturnAllGifts()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Setup category and donor - must exist before gift references them
            var category = new Category { Id = 1, Name = "Books" };
            var donor = new Donor
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Phone = "1234567890"
            };
            
            context.categories.Add(category);
            context.donors.Add(donor);
            context.SaveChanges();

            // Add gifts separately to ensure FK relationships are established
            context.gifts.Add(new Gift 
            { 
                Id = 1, 
                Name = "Gift1", 
                Description = "Desc1", 
                Image = "img1.jpg", 
                Price = 20, 
                CategoryId = 1, 
                DonorId = 1
            });
            context.gifts.Add(new Gift 
            { 
                Id = 2, 
                Name = "Gift2", 
                Description = "Desc2", 
                Image = "img2.jpg", 
                Price = 30, 
                CategoryId = 1, 
                DonorId = 1
            });
            context.SaveChanges();

            // Act
            var results = await service.GetGiftsAsync();

            // Assert
            Assert.NotEmpty(results);
            Assert.True(results.Any(g => g.Name == "Gift1"), "Gift1 should be in results");
            context.Dispose();
        }

        #endregion

        #region GetGiftById Tests

        [Fact]
        public async Task GetGiftById_WithValidId_ShouldReturnGift()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            var gift = new Gift
            {
                Id = 5,
                Name = "Test Gift",
                Description = "Test Description",
                Image = "test.jpg",
                Price = 50,
                CategoryId = 1,
                DonorId = 1,
                Orders = new List<Order>()
            };
            context.gifts.Add(gift);
            context.SaveChanges();

            // Act
            var result = await service.GetGiftById(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Gift", result.Name);
            context.Dispose();
        }

        [Fact]
        public async Task GetGiftById_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetGiftById(9999));
            context.Dispose();
        }

        [Fact]
        public async Task GetGiftById_WithZeroId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetGiftById(0));
            context.Dispose();
        }

        #endregion

        #region GetGiftByName Tests

        [Fact]
        public async Task GetGiftByName_WithValidName_ShouldReturnGift()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Setup category and donor
            var category = new Category { Id = 10, Name = "Electronics" };
            var donor = new Donor
            {
                Id = 10,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Phone = "9876543210"
            };
            
            context.categories.Add(category);
            context.donors.Add(donor);
            context.SaveChanges();

            // Add the gift
            context.gifts.Add(new Gift
            {
                Id = 10,
                Name = "Named Gift",
                Description = "A named gift",
                Image = "named.jpg",
                Price = 35,
                CategoryId = 10,
                DonorId = 10
            });
            context.SaveChanges();

            // Act
            var result = await service.GetGiftByName("Named Gift");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Named Gift", result.Name);
            context.Dispose();
        }

        [Fact]
        public async Task GetGiftByName_WithNullName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetGiftByName(null!));
            context.Dispose();
        }

        [Fact]
        public async Task GetGiftByName_WithNonExistentName_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetGiftByName("NonExistent"));
            context.Dispose();
        }

        #endregion

        #region UpdateGiftAsync Tests

        [Fact]
        public async Task UpdateGiftAsync_WithValidData_ShouldUpdateGiftSuccessfully()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            var gift = new Gift
            {
                Id = 15,
                Name = "Original Name",
                Description = "Original Description",
                Image = "original.jpg",
                Price = 25,
                CategoryId = 1,
                DonorId = 1,
                Orders = new List<Order>()
            };
            context.gifts.Add(gift);
            context.SaveChanges();

            var updateDto = new DtoGifts
            {
                Name = "Updated Name",
                Description = "Updated Description",
                CategoryId = 1,
                DonorId = 1,
                Price = 35,
                Image = "updated.jpg"
            };

            // Act
            var result = await service.UpdateGiftAsync(updateDto, 15);

            // Assert
            Assert.True(result);
            var updatedGift = context.gifts.FirstOrDefault(g => g.Id == 15);
            Assert.NotNull(updatedGift);
            Assert.Equal("Updated Name", updatedGift.Name);
            context.Dispose();
        }

        [Fact]
        public async Task UpdateGiftAsync_WithNullDto_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateGiftAsync(null!, 1));
            context.Dispose();
        }

        [Fact]
        public async Task UpdateGiftAsync_WithZeroId_ShouldThrowArgumentException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var updateDto = new DtoGifts
            {
                Name = "Test",
                Description = "Test",
                CategoryId = 1,
                DonorId = 1,
                Price = 20,
                Image = "test.jpg"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateGiftAsync(updateDto, 0));
            context.Dispose();
        }

        [Fact]
        public async Task UpdateGiftAsync_WithNonExistentId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();
            var updateDto = new DtoGifts
            {
                Name = "Test",
                Description = "Test",
                CategoryId = 1,
                DonorId = 1,
                Price = 20,
                Image = "test.jpg"
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateGiftAsync(updateDto, 9999));
            context.Dispose();
        }

        #endregion

        #region GetDonorsAsync Tests

        [Fact]
        public async Task GetDonorsAsync_WithValidGiftId_ShouldReturnDonor()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            var donor = new Donor
            {
                Id = 20,
                FirstName = "Gift",
                LastName = "Donor",
                Email = "donor@example.com",
                Phone = "5555555555",
                Donations = new List<Gift>()
            };
            context.donors.Add(donor);

            var gift = new Gift
            {
                Id = 20,
                Name = "Donor Gift",
                Description = "From donor",
                Image = "donor.jpg",
                Price = 45,
                CategoryId = 1,
                DonorId = 20,
                Donor = donor,
                Orders = new List<Order>()
            };
            context.gifts.Add(gift);
            context.SaveChanges();

            // Act
            var result = await service.GetDonorsAsync(20);

            // Assert
            Assert.NotNull(result);
            context.Dispose();
        }

        [Fact]
        public async Task GetDonorsAsync_WithInvalidGiftId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetDonorsAsync(9999));
            context.Dispose();
        }

        [Fact]
        public async Task GetDonorsAsync_WithZeroId_ShouldThrowArgumentException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetDonorsAsync(0));
            context.Dispose();
        }

        #endregion

        #region GetGiftByNOfUsers Tests

        [Fact]
        public async Task GetGiftByNOfUsers_WithNegativeNumber_ShouldThrowArgumentException()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetGiftByNOfUsers(-1));
            context.Dispose();
        }

        [Fact]
        public async Task GetGiftByNOfUsers_WithZeroUsers_ShouldReturnEmptyOrThrow()
        {
            // Arrange
            var (context, _, service) = CreateServiceWithContext();

            // Act - depends on repository implementation, may return empty list
            var result = await service.GetGiftByNOfUsers(0);

            // Assert
            Assert.NotNull(result);
            context.Dispose();
        }

        #endregion
    }
}
