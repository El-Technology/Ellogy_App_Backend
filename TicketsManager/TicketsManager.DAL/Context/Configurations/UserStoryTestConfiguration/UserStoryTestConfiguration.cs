using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models.UserStoryTests;

namespace TicketsManager.DAL.Context.Configurations.UserStoryTestConfiguration;

public class UserStoryTestConfiguration : IEntityTypeConfiguration<UserStoryTest>
{
    public void Configure(EntityTypeBuilder<UserStoryTest> builder)
    {
        builder.ToTable("UserStoryTest");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.TestScenarios);

        builder.HasOne(c => c.TestPlan)
            .WithOne(c => c.UserStoryTest)
            .HasForeignKey<TestPlan>(c => c.UserStoryTestId);

        builder.HasMany(c => c.TestCases)
            .WithOne(c => c.UserStoryTest)
            .HasForeignKey(c => c.UserStoryTestId);
    }
}