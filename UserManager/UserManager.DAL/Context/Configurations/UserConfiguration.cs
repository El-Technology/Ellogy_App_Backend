using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManager.DAL.Enums;
using UserManager.DAL.Models;

namespace UserManager.DAL.Context.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(20);
        builder.Property(c => c.Password)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(c => c.Organization)
            .HasMaxLength(100);
        builder.Property(c => c.Department)
            .HasMaxLength(100);
        builder.Property(c => c.AvatarLink);
        builder.Property(c => c.Salt)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(c => c.Role)
            .IsRequired()
            .HasDefaultValue(RoleEnum.User);
        builder.Property(c => c.TotalPointsUsage);
        builder.Property(c => c.TotalPurchasedPoints);
    }
}
