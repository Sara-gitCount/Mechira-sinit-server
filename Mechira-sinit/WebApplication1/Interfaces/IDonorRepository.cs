using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IDonorRepository
    {
        Task<List<Donor>> GetAllDonorsAsync();
        Task<bool> CreateDonorAsync(Donor donor);
        Task<bool> DeleteDonorAsync(int id);
        Task<bool> UpdateDonorAsync(Donor donor);
        Task<Donor> GetById(int id);
        Task<Donor> GetByName(string name);
        Task<Donor> GetByEmail(string email);
        Task<Donor> GetByGift(string giftName);
        Task<bool> AddDonation(int id,int giftId);

    }
}
