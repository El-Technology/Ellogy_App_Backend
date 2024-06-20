using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Context.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Title);
            builder.Property(a => a.Description);
            builder.Property(a => a.Sms);
            builder.Property(a => a.Email);
            builder.Property(a => a.Push);
            builder.HasOne(a => a.Ticket)
                .WithMany(a => a.Notifications)
                .HasForeignKey(a => a.TicketId);
        }
    }
}
