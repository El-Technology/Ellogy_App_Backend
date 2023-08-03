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
            builder.HasKey(a => a.TamplateName);
            builder.Property(a => a.Value)
                .IsRequired();
        }
    }
}
