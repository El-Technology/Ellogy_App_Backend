using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManager.DAL.Models;

namespace UserManager.DAL.Context.Configurations;

public class ForgotPasswordConfiguration : IEntityTypeConfiguration<ForgotPassword>
{
    public void Configure(EntityTypeBuilder<ForgotPassword> builder)
    {
        builder.ToTable("ForgotPassword");
        builder.HasKey(c => new { c.UserId, c.Token });
        builder.Property(c => c.IsValid)
            .IsRequired();
        builder.Property(c => c.ExpireDate)
            .IsRequired();
    }
}
