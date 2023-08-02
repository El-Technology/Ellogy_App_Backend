using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations
{
    public class TicketTableValueConfiguration : IEntityTypeConfiguration<TicketTableValue>
    {
        public void Configure(EntityTypeBuilder<TicketTableValue> builder)
        {
            builder.ToTable("TicketTableValues");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Value);

            builder.HasOne(t => t.TicketTable)
                .WithMany(t => t.TableValues)
                .HasForeignKey(t => t.TicketTableId)
                .IsRequired();
        }
    }
}

