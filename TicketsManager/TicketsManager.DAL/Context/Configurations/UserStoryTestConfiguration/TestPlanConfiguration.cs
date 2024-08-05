using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketsManager.DAL.Models.UserStoryTestsModels;

namespace TicketsManager.DAL.Context.Configurations.UserStoryTestConfiguration;

public class TestPlanConfiguration : IEntityTypeConfiguration<TestPlan>
{
    public void Configure(EntityTypeBuilder<TestPlan> builder)
    {
        builder.ToTable("TestPlan");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Objective);
        builder.Property(c => c.Scope);
        builder.Property(c => c.Resources);
        builder.Property(c => c.Schedule);
        builder.Property(c => c.TestEnvironment);
        builder.Property(c => c.RiskManagement);
    }
}