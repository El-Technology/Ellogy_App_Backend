using Microsoft.EntityFrameworkCore;
using UserManager.Common;
using UserManager.DAL.Context.Configurations;
using UserManager.DAL.Models;

namespace UserManager.DAL.Context;

public class UserManagerDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public UserManagerDbContext() { }

    public UserManagerDbContext(DbContextOptions<UserManagerDbContext> options) : base(options)
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(EnvironmentVariables.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
