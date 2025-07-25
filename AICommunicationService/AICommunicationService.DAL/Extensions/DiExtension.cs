﻿using AICommunicationService.DAL.Context;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AICommunicationService.DAL.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return services
                .AddDbContext<AICommunicationContext>(c => c.UseNpgsql(connectionString))
                .AddScoped<IAIPromptRepository, AIPromptRepository>();
        }
    }
}
