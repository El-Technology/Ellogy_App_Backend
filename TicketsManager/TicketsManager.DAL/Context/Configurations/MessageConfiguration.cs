using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Sender)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.Content)
            .IsRequired();
        builder.Property(c => c.SendTime)
            .IsRequired();
        builder.Property(c => c.ActionType);
        builder.Property(c => c.ActionState);

        builder.HasOne(c => c.Ticket)
            .WithMany(c => c.TicketMessages)
            .HasForeignKey(c => c.TicketId)
            .IsRequired();
    }
}
