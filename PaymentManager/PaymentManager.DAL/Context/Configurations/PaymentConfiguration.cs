using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Context.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status);
            builder.Property(x => x.UserEmail);
            builder.Property(x => x.ProductId);
            builder.Property(x => x.PaymentId);
            builder.Property(x => x.UpdatedBallance);
        }
    }
}
