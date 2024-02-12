using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AICommunicationService.DAL.Context.AiCommunication.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.TotalPointsUsage);
        builder.Property(a => a.TotalPurchasedPoints);
        builder.Property(a => a.Email);
        builder.Property(a => a.FirstName);
        builder.Property(a => a.LastName);
        builder.Property(a => a.AvatarLink);
    }
}