using Microsoft.EntityFrameworkCore;
using TicketsManager.Api.Middlewares;
using TicketsManager.BLL.Extensions;
using TicketsManager.Common;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);

AddServices(builder);

var app = builder.Build();

AddMiddleware(app);

app.MapControllers();
MigrateDatabase(app);

app.Run();

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();

    builder.Services.AddDataLayer(EnvironmentVariables.ConnectionString);
    builder.Services.AddBusinessLayer();
    builder.Services.AddMapping();
}

static void AddMiddleware(WebApplication app)
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

static void MigrateDatabase(IHost app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<TicketsManagerDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}
