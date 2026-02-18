using WebApplication1.Dto;
using WebApplication1.Models;
using static WebApplication1.Dto.DtoAuth;

namespace WebApplication1.Interfaces
{
    public interface IUsersService
    {
        Task<List<DtoUser>> GetAllUsersAsync();
        Task<DtoUser> GetUserByIdAsync(int id);
        Task<DtoUser> CreateUserAsync(User user);
        Task<DtoUser> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<LoginResponseDto?> AuthenticateAsync(string Email, string password);
        Task<bool> ExistingEmailAsync(string email);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<Basket>> Basket(int id);

    }
}
