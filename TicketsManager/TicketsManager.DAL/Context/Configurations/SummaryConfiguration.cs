using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations
{
    public class SummaryConfiguration : IEntityTypeConfiguration<TicketSummary>
    {
        public void Configure(EntityTypeBuilder<TicketSummary> builder)
        {
            builder.ToTable("TicketSummaries");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.IsPotential)
                .IsRequired();
            builder.Property(t => t.Data)
                .IsRequired();

            builder.HasOne(t => t.Ticket)
                .WithMany(t => t.TicketSummaries)
                .HasForeignKey(t => t.TicketId)
                .IsRequired();
        }
    }
}
