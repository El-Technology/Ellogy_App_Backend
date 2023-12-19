using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManager.DAL.Models;

namespace UserManager.DAL.Context.UserContext.Configurations;

public class ForgotPasswordConfiguration : IEntityTypeConfiguration<ForgotPassword>
{
    public void Configure(EntityTypeBuilder<ForgotPassword> builder)
    {
        builder.ToTable("ForgotPassword");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.IsValid)
            .IsRequired();
        builder.Property(c => c.Token)
            .IsRequired();
        builder.Property(c => c.UserId)
            .IsRequired();
        builder.Property(c => c.ExpireDate)
            .IsRequired();
    }
}
