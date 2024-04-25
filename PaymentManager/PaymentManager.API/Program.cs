using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaymentManager.API.Middlewares;
using PaymentManager.BLL.Extensions;
using PaymentManager.BLL.Hubs;
using PaymentManager.Common.Options;
using PaymentManager.DAL.Context;
using PaymentManager.DAL.Extensions;
using Serilog;
using Stripe;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("../Logs/log.txt")
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog(logger);

await AddServicesAsync(builder);

StripeConfiguration.ApiKey = await EnvironmentVariables.SecretKey;

var app = builder.Build();

app.UseSerilogRequestLogging();

MigrateDatabase(app);
AddMiddleware(app);

app.MapControllers();

app.Run();

static async Task AddServicesAsync(WebApplicationBuilder builder)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.SetIsOriginAllowed(origin => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    builder.Services.AddSignalR();
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(async options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JwtOptions.Issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = JwtOptions.GetSymmetricSecurityKey(
                    await EnvironmentVariables.JwtSecretKey),
                ValidateIssuerSigningKey = true
            };
        });

    builder.Services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Ellogy. Payment Manager service API",
            Version = "v1"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Just paste token value in field.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });
    });

    builder.Services.RegisterHttpClients();
    builder.Services.AddBusinessLayer(
        await EnvironmentVariables.AzureServiceBusConnectionStringPayment);

    builder.Services.AddMapping();
    builder.Services.AddDataLayer(
        await EnvironmentVariables.ConnectionString);
}

static void MigrateDatabase(IHost app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<PaymentContext>();
    if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
}

static void AddMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();
    app.UseWebSockets();

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHub<PaymentHub>("/paymentHub");
    app.UseMiddleware<ExceptionHandlerMiddleware>();
}