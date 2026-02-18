using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IGiftRepository
    {
        Task<List<Gift>> GetGiftsAsync();
        Task<bool> DeleteGiftAsync(int id);
        Task<bool> UpdateGiftAsync(Gift gift);
        Task<bool> AddGift(Gift gift);
        Task<Donor> GetDonorsAsync(int idGift);
        Task<Gift> GetGiftByName(string name);
        Task<List<Gift>> GetGiftByDonor(string donorName);
        Task<List<Gift>> GetGiftByNOfUsers(int nOfUsers);
        Task<Gift> GetById(int id);
        Task<List<Gift>> GetOrderByPrice_Category();
    }
}
