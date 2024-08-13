using System.Collections.Immutable;
using web_rest.Models;

namespace web_rest.Services
{
    public interface IUserService
    {
        Task<List<UserDetails>> GetUsersAsync();
        Task<UserDetails?> FindByIdAsync(int id);
        Task<UserDetails?> FindByEmailAndPassword(string email, string password);
        Task<UserDetails?> SaveUserAsync(UserDetails user);
        Task<UserDetails?> UpdateUserAsync(UserDetails user);
        Task<UserDetails?> DeleteUserByIdAsync(int id);

    }
}
