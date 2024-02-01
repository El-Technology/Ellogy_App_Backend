using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Context.PaymentContext.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status);
            builder.Property(x => x.UserEmail);
            builder.Property(x => x.AmountOfPoints);
            builder.Property(x => x.PaymentId);
            builder.Property(x => x.InvoiceId);
            builder.Property(x => x.Mode);
            builder.Property(x => x.ProductName);
            builder.Property(x => x.UpdatedBallance);
            builder.Property(x => x.UserId);
        }
    }
}
