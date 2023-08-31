using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context.Configurations
{
    public class TicketDiagramConfiguration : IEntityTypeConfiguration<TicketDiagram>
    {
        public void Configure(EntityTypeBuilder<TicketDiagram> builder)
        {
            builder.ToTable("TicketDiagrams");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title)
                .IsRequired();
            builder.Property(t => t.Description)
                .IsRequired();
            builder.Property(t => t.PictureLink)
                .IsRequired();
            builder.Property(t => t.PictureLinkPng);

            builder.HasOne(a => a.Usecase)
                .WithMany(a => a.Diagrams)
                .HasForeignKey(a => a.UsecaseId);
        }
    }
}
