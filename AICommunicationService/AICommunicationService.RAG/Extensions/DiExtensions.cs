using AICommunicationService.RAG.Context.Vector;
using AICommunicationService.RAG.Interfaces;
using AICommunicationService.RAG.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pgvector.EntityFrameworkCore;

namespace AICommunicationService.RAG.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddRAGDataLayer(this IServiceCollection services, string vectorConnectionString)
    {
        return services
            .AddDbContext<VectorContext>(c => c.UseNpgsql(vectorConnectionString, o => o.UseVector()))
            .AddScoped<IEmbeddingRepository, EmbeddingRepository>()
            .AddScoped<IDocumentRepository, DocumentRepository>()
            .AddScoped<IDocumentSharingRepository, DocumentSharingRepository>();
    }
}