using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserManager.Api.Middlewares;
using UserManager.BLL.Extensions;
using UserManager.Common;
using UserManager.DAL.Context;
using UserManager.DAL.Extensions;

namespace UserManager.Api
{
    //TODO add health checks
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            AddServices(builder);

            var app = builder.Build();

            AddMiddleware(app);

            app.MapControllers();
            MigrateDatabase(app);

            app.Run();
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new()
                {
                    Title = "Ellogy. User Manager service API",
                    Version = "v1",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddHealthChecks();

            builder.Services.AddDataLayer(EnvironmentVariables.ConnectionString);
            builder.Services.AddBusinessLayer();
            builder.Services.AddMapping();
        }

        private static void AddMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseHealthChecks("/health");
            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlerMiddleware>();
        }

        private static void MigrateDatabase(IHost app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<UserManagerDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
    }
}