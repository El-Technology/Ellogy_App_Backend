using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models.UsecaseModels;
using TicketsManager.DAL.Models.UserStoryTestsModels;

namespace TicketsManager.DAL.Context.Configurations;

public class UsecaseConfiguration : IEntityTypeConfiguration<Usecase>
{
    public void Configure(EntityTypeBuilder<Usecase> builder)
    {
        builder.ToTable("Usecases");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title);
        builder.Property(t => t.Description);
        builder.Property(t => t.Order);

        builder.HasOne(a => a.Ticket)
            .WithMany(a => a.Usecases)
            .HasForeignKey(a => a.TicketId);

        builder.HasOne(a => a.UserStoryTest)
            .WithOne(a => a.Usecase)
            .HasForeignKey<UserStoryTest>(a => a.UsecaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.TicketSummaries)
            .WithMany(a => a.Usecases);
    }
}
