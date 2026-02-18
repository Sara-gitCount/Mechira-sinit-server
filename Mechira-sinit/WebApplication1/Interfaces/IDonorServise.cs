using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IDonorServise
    {
        Task<List<DtoDonors>> GetAllDonors();
        Task<bool> CreateDonorAsync(DonorCreateDto donor);
        Task<bool> UpdateDonorAsync(DtoDonors donor, int id);
        Task<bool> DeleteDonorAsync(int id);
        Task<DonorCreateDto> GetByName(string name);
        Task<DtoDonors> GetByEmail(string email);
        Task<DtoDonors> GetByGift(string giftName);
        Task<bool> AddDonation(int id, int giftId);
        Task<Donor> GetById(int id);

    }
}
