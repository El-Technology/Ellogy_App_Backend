using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations;

public class TicketTableConfiguration : IEntityTypeConfiguration<TicketTable>
{
    public void Configure(EntityTypeBuilder<TicketTable> builder)
    {
        builder.ToTable("TicketTables");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Table);

        builder.HasOne(a => a.Usecase)
            .WithMany(a => a.Tables)
            .HasForeignKey(t => t.UsecaseId);
    }
}