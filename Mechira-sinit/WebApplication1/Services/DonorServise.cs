using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class DonorServise: IDonorServise
    {
        private readonly IDonorRepository donorsRepository;
        private readonly ILogger<DonorServise> logger;
        public DonorServise(IDonorRepository donorsRepository, ILogger<DonorServise> logger)
        {
            this.donorsRepository = donorsRepository;
            this.logger = logger;
        }
        public async Task<bool> CreateDonorAsync(DonorCreateDto donor)
        {
            if (donor == null)
            {
                logger.LogError("Donor object is null");
                throw new ArgumentNullException(nameof(donor));
            }           
            var d = await donorsRepository.CreateDonorAsync(MapToResponseDonor(donor));
            if (!d)
            {
                logger.LogError("Failed to create donor");
                throw new Exception("Failed to create donor");
            }
            logger.LogInformation($"Donor {donor.FirstName} {donor.LastName} created successfully");
            return d;
        }

        public async Task<bool> DeleteDonorAsync(int id)
        {
            if (id == 0)
            {
                logger.LogError("Invalid donor ID");
                throw new ArgumentException("Invalid donor ID", nameof(id));
            }
            var d = await donorsRepository.DeleteDonorAsync(id);

            if (!d)
            {
                logger.LogError($"Donor with id {id} not found");
                throw new KeyNotFoundException($"Donor with id {id} not found");
            }
            logger.LogInformation($"Donor with id {id} deleted successfully");
            return d;
        }

        public async Task<List<DtoDonors>> GetAllDonors()
        {

            var donors = await donorsRepository.GetAllDonorsAsync();

            if (donors == null)
            {
                logger.LogWarning("No donors found");
            }
            return donors.Select(MapToResponseDto).ToList();
        }
        public async Task<Donor> GetById(int id)
        {
            var d = await donorsRepository.GetById(id);
            if (d == null)
            {
                logger.LogWarning("No donors found");
                throw new KeyNotFoundException($"Donor with id {id} not found");
            }
            return d;
                
        }
        public async Task<bool> UpdateDonorAsync(DtoDonors donor, int id)
        {
            if (donor == null)
            {
                logger.LogWarning("Donor object is null");
                throw new ArgumentNullException(nameof(donor));
            }
            if (id == 0)
            {
                logger.LogError("Invalid donor ID");
                throw new ArgumentException("Invalid donor ID", nameof(id));
            }
            var d = await donorsRepository.GetById(id);

            if (d == null)
            {
                logger.LogError($"Donor with id {id} not found");
                throw new KeyNotFoundException($"Donor with id {id} not found");
            }
            d.FirstName = donor.FirstName;
            d.LastName = donor.LastName;
            d.Phone = donor.Phone;
            d.Email = donor.Email;
            var dn = await donorsRepository.UpdateDonorAsync(d);
            if (!dn)
            {
                logger.LogError($"Failed to update donor with id {id}");
                throw new KeyNotFoundException($"Failed to update donor with id {id}");
            }
            logger.LogInformation($"Donor with id {id} updated successfully");
            return dn;
        }

        public async Task<DonorCreateDto> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                logger.LogError("Name cannot be null or empty");
            }

            var d = await donorsRepository.GetByName(name);

            if (d == null)
            {
                logger.LogError($"Donor with name {name} not found");
                throw new KeyNotFoundException($"Donor with name {name} not found");
            }

            return MapToResponseCDto(d);
        }

        public async Task<DtoDonors> GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                logger.LogError("Email cannot be null or empty");
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }

            var d = await donorsRepository.GetByEmail(email);

            if (d == null)
            {
                logger.LogError($"Donor with email {email} not found");
                throw new KeyNotFoundException($"Donor with email {email} not found");
            }

            return MapToResponseDto(d);
        }

        public async Task<DtoDonors> GetByGift(string giftName)
        {
            if (giftName == null)
            {
                logger.LogError("Invalid gift ID");
                throw new ArgumentException("Invalid gift ID", nameof(giftName));
            }

            var d = await donorsRepository.GetByGift(giftName);

            if (d == null)
            {
                logger.LogError($"Donor with gift name {giftName} not found");
                throw new KeyNotFoundException($"Donor with gift name {giftName} not found");
            }

            return MapToResponseDto(d);
        }
        public async Task<bool> AddDonation(int id, int giftId)
        {
            if (id <= 0)
            {
                logger.LogError("Invalid donor ID");
                throw new ArgumentException("Invalid donor ID", nameof(id));
            }
            if (giftId <=0)
            {
                logger.LogError("Invalid gift id");
                throw new ArgumentException("Invalid gift ID", nameof(giftId));
            }
            var g =await donorsRepository.AddDonation(id, giftId);
            if(!g)
            {
                logger.LogError($"Failed to add donation to donor with id {id}");
                throw new Exception($"Failed to add donation to donor with id {id}");
            }
            return true;
        }
        private static DtoDonors MapToResponseDto(Donor donor)
        {
            return new DtoDonors
            {
                FirstName = donor.FirstName,
                LastName = donor.LastName,
                Donations = {},// donor.Donations,
                Phone = donor.Phone,
                Email = donor.Email,
            };
        }
        private static DonorCreateDto MapToResponseCDto(Donor donor)
        {
            return new DonorCreateDto
            {
                Id = donor.Id,
                FirstName = donor.FirstName,
                LastName = donor.LastName,
                Phone = donor.Phone,
                Email = donor.Email,
            };
        }
        private static Donor MapToResponseDonor(DonorCreateDto donor)
        {
            return new Donor
            {
                Id = donor.Id,
                FirstName = donor.FirstName,
                LastName = donor.LastName,
                Donations = { },// donor.Donations,
                Phone = donor.Phone,
                Email = donor.Email,
            };
        }

      
    }
}
