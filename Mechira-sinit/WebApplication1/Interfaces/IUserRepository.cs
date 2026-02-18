using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IUsersRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<Gift>> Basket(int idUser);
    }
}
