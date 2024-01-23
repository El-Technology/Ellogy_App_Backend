using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations
{
    public class UsecaseConfiguration : IEntityTypeConfiguration<Usecase>
    {
        public void Configure(EntityTypeBuilder<Usecase> builder)
        {
            builder.ToTable("Usecases");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title);
            builder.Property(t => t.Description);

            builder.HasOne(a => a.Ticket)
                .WithMany(a => a.Usecases)
                .HasForeignKey(a => a.TicketId);
        }
    }
}
