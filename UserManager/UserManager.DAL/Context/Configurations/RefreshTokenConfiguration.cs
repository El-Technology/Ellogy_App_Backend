using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManager.DAL.Models;

namespace UserManager.DAL.Context.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ExpireDate)
            .IsRequired();
        builder.Property(c => c.Value)
            .IsRequired();
        builder.Property(c => c.IsValid)
            .IsRequired();
        builder.HasOne(c => c.User)
            .WithOne(c => c.RefreshToken)
            .HasForeignKey<RefreshToken>(c => c.UserId)
            .IsRequired();
    }
}
