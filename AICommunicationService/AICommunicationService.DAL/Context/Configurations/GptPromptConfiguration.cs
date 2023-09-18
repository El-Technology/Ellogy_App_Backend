using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AICommunicationService.DAL.Context.Configurations
{
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
        }
    }
}
