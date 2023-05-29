using Microsoft.EntityFrameworkCore;
using UserManager.DAL.Context.Configurations;
using UserManager.DAL.Models;

namespace UserManager.DAL.Context;

public class UserManagerDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public UserManagerDbContext(DbContextOptions<UserManagerDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
