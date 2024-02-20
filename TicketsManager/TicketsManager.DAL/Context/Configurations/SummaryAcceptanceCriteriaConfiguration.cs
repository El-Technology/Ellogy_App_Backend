using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations;

public class SummaryAcceptanceCriteriaConfiguration : IEntityTypeConfiguration<SummaryAcceptanceCriteria>
{
    public void Configure(EntityTypeBuilder<SummaryAcceptanceCriteria> builder)
    {
        builder.ToTable("SummaryAcceptanceCriteria");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title);
        builder.Property(t => t.Description);

        builder.HasOne(t => t.TicketSummary)
            .WithMany(t => t.SummaryAcceptanceCriteria)
            .HasForeignKey(t => t.TicketSummaryId)
            .IsRequired();
    }
}