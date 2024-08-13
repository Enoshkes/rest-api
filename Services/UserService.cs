using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using web_rest.Data;
using web_rest.Models;

namespace web_rest.Services
{
    public class UserService(UsersDbContext context) : IUserService
    {
        public async Task<UserDetails?> DeleteUserByIdAsync(int id)
        {
            var user = await FindByIdAsync(id)
                ?? throw new Exception($"User by the id {id} doesnt exists");

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<UserDetails?> FindByEmailAndPassword(string email, string password) =>
            await context.Users.FirstOrDefaultAsync(user =>
                user.Email == email &&
                user.Password == password
            );

        public async Task<UserDetails?> FindByIdAsync(int id) =>
            await context.Users.FindAsync(id);

        public async Task<List<UserDetails>> GetUsersAsync() =>
            await context.Users.ToListAsync();

        public async Task<UserDetails?> SaveUserAsync(UserDetails user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<UserDetails?> UpdateUserAsync(UserDetails user)
        {
            var toUpdate = await FindByIdAsync(user.Id) ??
                throw new Exception($"User by the id {user.Id} doesnt exists");

            toUpdate.Email = user.Email;
            toUpdate.Password = user.Password;
            await context.SaveChangesAsync();
            return toUpdate;
        }
    }
}
