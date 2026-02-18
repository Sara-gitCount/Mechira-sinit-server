using System.ComponentModel.DataAnnotations;
using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Repository;

namespace WebApplication1.Services
{
    public class GiftService: IGiftService
    {
        private readonly IGiftRepository giftRepository;
        private readonly ILogger<GiftService> logger;
        public GiftService(IGiftRepository giftRepository, ILogger<GiftService> logger)
        {
            this.giftRepository = giftRepository;
            this.logger = logger;
        }
        public async Task<bool> AddGift(DtoGifts gift)
        {
            if (gift == null)
            {
                logger.LogError("Gift object is null");
                throw new ArgumentNullException(nameof(gift));
            }
            var newGift = new Gift();
            newGift.Name = gift.Name;
            newGift.Description = gift.Description;
            newGift.CategoryId = gift.CategoryId;
            newGift.DonorId = gift.DonorId;
            newGift.Price = gift.Price;
            newGift.Image = gift.Image;

            var g = await giftRepository.AddGift(newGift);

            if (!g)
            {
                logger.LogError("Error adding gift to the repository");
                throw new Exception("Error adding gift");
            }
            logger.LogInformation("Gift added successfully");
            return g;
        }

        public async Task<bool> DeleteGiftAsync(int id)
        {
            if (id == 0)
            {
                logger.LogError("Invalid gift ID");
                throw new ArgumentException("Invalid gift ID", nameof(id));
            }
            var g = await giftRepository.DeleteGiftAsync(id);

            if (!g)
            {
                logger.LogError($"Gift with id {id} not found or could not be deleted");
                throw new KeyNotFoundException($"Gift with id {id} not found or could not be deleted");
            }
            logger.LogInformation($"Gift with id {id} deleted successfully");
            return g;
        }

        public async Task<DtoDonors> GetDonorsAsync(int idGift)
        {
            if (idGift == 0)
            {
                logger.LogError("Invalid gift ID");
                throw new ArgumentException("Invalid gift ID", nameof(idGift));
            }
            var donor = await giftRepository.GetDonorsAsync(idGift);
            if (donor == null)
            {
                logger.LogError($"Donor for gift id {idGift} not found");
                throw new KeyNotFoundException($"Donor for gift id {idGift} not found");
            }
            return MapToResponseDto(donor);
        }

        public async Task<List<DtoGifts_D>> GetGiftByDonor(string donorName)
        {
            if (donorName == null)
            {
                logger.LogError("Donor name is null");
                throw new ArgumentNullException(nameof(donorName));
            }
            var gifts = await giftRepository.GetGiftByDonor(donorName);
            if (gifts == null)
            {
                logger.LogWarning($"Gifts for donor {donorName} not found");
                throw new KeyNotFoundException($"Gifts for donor {donorName} not found");
            }
            var g = gifts.Select(MapToResponseDto_d).ToList();
            if (g.Count == 0)
            {
                logger.LogWarning($"Gifts for donor {donorName} not found");
                throw new KeyNotFoundException($"Gifts for donor {donorName} not found");
            }
            return g;
        }

        public async Task<DtoGift> GetGiftByName(string name)
        {
            if (name == null)
            {
                logger.LogError("Gift name is null");
                throw new ArgumentNullException(nameof(name));
            }
            var gifts = await giftRepository.GetGiftByName(name);
            if (gifts == null)
            {
                logger.LogWarning($"Gift with name {name} not found");
                throw new KeyNotFoundException($"Gift with name {name} not found");
            }
            return MapToResponseDtog(gifts);
        }

        public async Task<List<DtoGifts_D>> GetGiftByNOfUsers(int nOfUsers)
        {
            if (nOfUsers < 0)
            {
                logger.LogError("Number of users must be greater than zero");
                throw new ArgumentException("Number of users must be greater than zero", nameof(nOfUsers));
            }
            var gifts = await giftRepository.GetGiftByNOfUsers(nOfUsers);
            if (gifts == null || gifts.Count == 0)
            {
                logger.LogWarning($"No gifts found for {nOfUsers} users");
            }
            return gifts.Select(MapToResponseDto_d).ToList();
        }

        public async Task<List<DtoGift>> GetGiftsAsync()
        {
            var gifts = await giftRepository.GetGiftsAsync();
            if (gifts == null || gifts.Count == 0)
            {
                logger.LogWarning("No gifts found");
            }
            var g = gifts.Select(MapToResponseDtog).ToList();
            if (g.Count == 0)
            {
                logger.LogError("Field to map to dto");
                throw new KeyNotFoundException("No gifts found");
            }
            return g;
        }

        public async Task<bool> UpdateGiftAsync(DtoGifts gift, int id)
        {
            if (gift == null)
            {
                logger.LogError("Gift object is null");
                throw new ArgumentNullException(nameof(gift));
            }
            if (id == 0)
            {
                logger.LogError("Invalid gift ID");
                throw new ArgumentException("Invalid gift ID", nameof(id));
            }
            var g = await giftRepository.GetById(id);
            if (g == null)
            {
                logger.LogError($"Gift with id {id} not found");
                throw new KeyNotFoundException($"Gift with id {id} not found");
            }
            if (g != null)
            {
                g.Id = id;
                g.Name = gift.Name;
                g.Description = gift.Description;
                g.CategoryId = gift.CategoryId;
                g.Price = gift.Price;
                g.DonorId = gift.DonorId;
                g.Image = gift.Image;
            }
            var ug = await giftRepository.UpdateGiftAsync(g);
            if (!ug)
            {
                logger.LogError("Error updating gift");
                throw new Exception("Error updating gift");
            }
            logger.LogInformation($"Gift with id {id} updated successfully");
            return ug;
        }
        public async Task<DtoGifts> GetGiftById(int id)
        {
            if(id == 0)
            {
                logger.LogError($"{nameof(GetGiftById)}");
                throw new ArgumentNullException(nameof(id));
            }
            logger.LogInformation($"{nameof(GetGiftById)}");
            var g = await giftRepository.GetById(id);
            if (g == null)
            {
                logger.LogError($"Gift with id {id} not found");
                throw new KeyNotFoundException($"Gift with id {id} not found");
            }
            return MapToResponseDto(g);

        }
        private static DtoGifts_D MapToResponseDto_d(Gift gifts) => new DtoGifts_D
        {
            Name = gifts.Name,
            Description = gifts.Description,
            Image = gifts.Image,
            CategoryId = gifts.CategoryId,
        };
        private static DtoGifts MapToResponseDto(Gift gifts) => new DtoGifts
        {
            Name = gifts.Name,
            Description = gifts.Description,
            Image = gifts.Image,
            CategoryId = gifts.CategoryId,
            DonorId = gifts.DonorId,
            Price = gifts.Price,
        };
        private static DtoGift MapToResponseDtog(Gift gifts) => new DtoGift
        {
            Id = gifts.Id,
            Name = gifts.Name,
            Description = gifts.Description,
            Image = gifts.Image,
            CategoryName = gifts.Category.Name,
            DonorId = gifts.DonorId,
            Price = gifts.Price,
        };
        private static DtoDonors MapToResponseDto(Donor donor)
        {
            return new DtoDonors
            {
                FirstName = donor.FirstName,
                LastName = donor.LastName,
                //Donations = donor.Donations,// donor.Donations,
                Phone = donor.Phone,
                Email = donor.Email,
            };
        }

        public async Task<List<DtoGift>> GetOrderByPrice_Category()
        {
            var gifts = await giftRepository.GetOrderByPrice_Category();
            if (gifts == null)
                logger.LogWarning("No gifts found in the repository.");
            return gifts.Select(g => new DtoGift
            {
                Name = g.Name,
                Description = g.Description,
                Image = g.Image,
                Price = g.Price,
                DonorId = g.DonorId,
                CategoryName = g.Category.Name,
            }
            ).ToList();
        }

       
    }
}
