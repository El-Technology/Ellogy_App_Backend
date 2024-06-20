using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Context.Configurations;
public class TicketShareConfiguration : IEntityTypeConfiguration<TicketShare>
{
    public void Configure(EntityTypeBuilder<TicketShare> builder)
    {
        builder.ToTable("TicketShares");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.TicketId)
            .IsRequired();
        builder.Property(c => c.SharedUserId)
            .IsRequired();
        builder.Property(c => c.Permission)
            .IsRequired();
        builder.Property(c => c.TicketCurrentStep);
        builder.Property(c => c.SubStageEnum);
        builder.Property(c => c.GivenAt)
            .IsRequired();
        builder.Property(c => c.RevokedAt);

        builder.HasOne(c => c.Ticket)
            .WithMany(c => c.TicketShares)
            .HasForeignKey(c => c.TicketId);
    }
}