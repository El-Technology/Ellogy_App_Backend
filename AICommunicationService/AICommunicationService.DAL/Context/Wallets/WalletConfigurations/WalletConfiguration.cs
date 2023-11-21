using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AICommunicationService.DAL.Context.Wallets.WalletConfigurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId);
            builder.Property(x => x.Balance);
        }
    }
}
