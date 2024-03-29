using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(250);
        builder.Property(c => c.Description);
        builder.Property(c => c.Status)
            .IsRequired();
        builder.Property(c => c.CurrentStep)
            .IsRequired();
        builder.Property(c => c.CreatedDate)
            .IsRequired();
        builder.Property(c => c.UpdatedDate);
        builder.Property(c => c.Comment);
        builder.Property(c => c.Context);
        builder.Property(c => c.BannersJson);
    }
}
