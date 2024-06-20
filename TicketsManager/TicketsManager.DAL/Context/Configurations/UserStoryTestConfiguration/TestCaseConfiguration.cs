using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models.UserStoryTestsModels;

namespace TicketsManager.DAL.Context.Configurations.UserStoryTestConfiguration;

public class TestCaseConfiguration : IEntityTypeConfiguration<TestCase>
{
    public void Configure(EntityTypeBuilder<TestCase> builder)
    {
        builder.ToTable("TestCase");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.TestCaseId);
        builder.Property(c => c.Description);
        builder.Property(c => c.PreConditions);
        builder.Property(c => c.TestSteps);
        builder.Property(c => c.TestData);
        builder.Property(c => c.ExpectedResult);
    }
}