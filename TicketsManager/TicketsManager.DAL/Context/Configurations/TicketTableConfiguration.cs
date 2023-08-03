using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations
{
    public class TicketTableConfiguration : IEntityTypeConfiguration<TicketTable>
    {
        public void Configure(EntityTypeBuilder<TicketTable> builder)
        {
            builder.ToTable("TicketTables");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TableKey)
                .IsRequired();

            builder.HasMany(t => t.TableValues)
                .WithOne(t => t.TicketTable)
                .HasForeignKey(t => t.TicketTableId)
                .IsRequired();

            builder.HasOne(t => t.Ticket)
                .WithMany(t => t.TicketTables)
                .HasForeignKey(t => t.TicketId);
        }
    }

}
