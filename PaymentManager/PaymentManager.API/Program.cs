using Microsoft.EntityFrameworkCore;
using PaymentManager.API.Middlewares;
using PaymentManager.BLL.Extensions;
using PaymentManager.DAL.Context;
using PaymentManager.DAL.Extensions;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

AddServices(builder);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("SECRET_KEY");


var app = builder.Build();
MigrateDatabase(app);
AddMiddleware(app);

app.MapControllers();

app.Run();

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddBusinessLayer();
    builder.Services.AddDataLayer(Environment.GetEnvironmentVariable("CONNECTIONSTRING_PAYMENT"));
}

static void MigrateDatabase(IHost app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<PaymentContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

static void AddMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();

    app.UseMiddleware<ExceptionHandlerMiddleware>();
}
