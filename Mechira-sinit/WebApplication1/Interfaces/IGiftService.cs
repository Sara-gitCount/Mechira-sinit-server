using WebApplication1.Dto;

namespace WebApplication1.Interfaces
{
    public interface IGiftService
    {
        Task<List<DtoGift>> GetGiftsAsync();
        Task<bool> DeleteGiftAsync(int id);
        Task<bool> UpdateGiftAsync(DtoGifts gift, int id);
        Task<bool> AddGift(DtoGifts gift);
        Task<DtoDonors> GetDonorsAsync(int idGift);
        Task<DtoGift> GetGiftByName(string name);
        Task<List<DtoGifts_D>> GetGiftByDonor(string donorName);
        Task<List<DtoGifts_D>> GetGiftByNOfUsers(int nOfUsers);
        Task<List<DtoGift>> GetOrderByPrice_Category();
        Task <DtoGifts> GetGiftById(int id);

    }
}
