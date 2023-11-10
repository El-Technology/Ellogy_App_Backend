using Microsoft.EntityFrameworkCore;
using PaymentManager.Common;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Context.UserContext
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public UserContext() { }
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(EnvironmentVariables.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserContext).Assembly);
        }
    }
}
