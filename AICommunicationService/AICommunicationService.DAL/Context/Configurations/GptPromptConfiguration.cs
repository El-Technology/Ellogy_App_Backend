using AICommunicationService.Common.Helpers;
using AICommunicationService.DAL.Models;
using AICommunicationService.DAL.Models.Roots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AICommunicationService.DAL.Context.Configurations;

public class GptPromptConfiguration : IEntityTypeConfiguration<AIPrompt>
{
    public void Configure(EntityTypeBuilder<AIPrompt> builder)
    {
        builder.ToTable("AIPrompts");
        builder.HasKey(a => a.TemplateName);
        builder.Property(a => a.Value)
            .IsRequired();
        builder.Property(a => a.Input);
        builder.Property(a => a.Description);
        builder.Property(a => a.Functions);
        builder.Property(a => a.JsonSample);
        builder.HasData(
            DbSeedHelper.ConvertJsonToList<AIPromptsRoot>
            ("AIPrompts_Seed.json", ".DAL")!.AIPrompts!);
    }
}
