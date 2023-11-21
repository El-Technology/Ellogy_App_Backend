using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Context.UserContext.Configuration
{
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
            builder.Property(c => c.Role)
                .IsRequired()
                .HasDefaultValue(RoleEnum.User);
            builder.Property(x => x.TotalPurchasedTokens);
        }
    }
}
