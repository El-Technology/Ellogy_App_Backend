using Microsoft.EntityFrameworkCore;
using UserManager.Common;
using UserManager.DAL.Models;

namespace UserManager.DAL.Context;

public class UserManagerDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<ForgotPassword> ForgotPasswords { get; set; } = null!;

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserManagerDbContext).Assembly);
    }
}
