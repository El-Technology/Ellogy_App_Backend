using AICommunicationService.RAG.Context.Vector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pgvector.EntityFrameworkCore;

namespace AICommunicationService.RAG.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddRAGLayer(this IServiceCollection services, string vectorConnectionString)
        {
            return services
                .AddDbContext<VectorContext>(c => c.UseNpgsql(vectorConnectionString, o => o.UseVector()));
        }
    }
}
