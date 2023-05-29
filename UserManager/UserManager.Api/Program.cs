using UserManager.Common;
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
            app.Run();
        }


        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDataLayer(EnvironmentVariables.ConnectionString);
        }

        private static void AddMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
        }
    }
}