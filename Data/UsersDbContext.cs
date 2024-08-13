using Microsoft.EntityFrameworkCore;
using web_rest.Models;

namespace web_rest.Data
{
    public class UsersDbContext(
        DbContextOptions<UsersDbContext> options,
        IConfiguration configuration
    ): DbContext(options)
    {
        private readonly string CONNECTION_STRING = "DefaultConnection";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString(CONNECTION_STRING));
        }
        public DbSet<UserDetails> Users { get; set; }
    }
}
