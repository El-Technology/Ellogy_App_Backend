using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations;

public class SummaryScenarioConfiguration : IEntityTypeConfiguration<SummaryScenario>
{
    public void Configure(EntityTypeBuilder<SummaryScenario> builder)
    {
        builder.ToTable("SummaryScenario");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title);
        builder.Property(t => t.Description);

        builder.HasOne(t => t.TicketSummary)
            .WithMany(t => t.SummaryScenarios)
            .HasForeignKey(t => t.TicketSummaryId)
            .IsRequired();
    }
}