using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Context.Configurations;

public class ActionHistoryConfiguration : IEntityTypeConfiguration<ActionHistory>
{
    public void Configure(EntityTypeBuilder<ActionHistory> builder)
    {
        builder.ToTable("ActionHistories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.TicketId)
            .IsRequired();
        builder.Property(c => c.ActionHistoryEnum)
            .IsRequired();
        builder.Property(c => c.TicketCurrentStepEnum)
            .IsRequired();
        builder.Property(c => c.UserEmail);
        builder.Property(c => c.OldValue);
        builder.Property(c => c.NewValue);
        builder.Property(c => c.ActionTime)
            .IsRequired();

        builder.HasOne(c => c.Ticket)
            .WithMany(c => c.ActionHistories)
            .HasForeignKey(c => c.TicketId);
    }
}
