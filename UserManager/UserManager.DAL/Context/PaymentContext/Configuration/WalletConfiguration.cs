using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManager.DAL.Models;

namespace UserManager.DAL.Context.PaymentContext.Configuration
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");
            builder.HasKey(x => x.Id);
            builder.Property(p => p.UserId);
            builder.Property(p => p.Balance);
        }
    }
}
