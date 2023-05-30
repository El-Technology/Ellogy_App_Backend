using Microsoft.EntityFrameworkCore;
using UserManager.BLL.Extensions;
using UserManager.Common;
using UserManager.DAL.Context;
using UserManager.DAL.Extensions;

namespace UserManager.Api
{
    public class Program
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
            builder.Services.AddSwaggerGen();
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