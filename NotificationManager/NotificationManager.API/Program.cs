using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NotificationManager.API.Infrastructure;
using NotificationManager.BLL.Extensions;
using NotificationManager.Common;
using NotificationManager.Common.Options;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureLogging(builder);

await ConfigureServicesAsync(builder);

var app = builder.Build();

ConfigureMiddleware(app);

app.Run();

static void ConfigureLogging(WebApplicationBuilder builder)
{
    var logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);
    builder.Host.UseSerilog(logger);
}

static async Task ConfigureServicesAsync(WebApplicationBuilder builder)
{
    var services = builder.Services;

    services.AddExceptionHandler<GlobalExceptionHandler>();
    services.AddProblemDetails();

    services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.SetIsOriginAllowed(origin => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    services.AddAuthorization();
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

    services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Ellogy. Notification Manager service API",
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
                    },
                },
                new List<string>()
            }
        });
    });

    services.AddBusinessLayer(
        await EnvironmentVariables.AzureServiceBusConnectionString,
        await EnvironmentVariables.EmailClientConnectionString,
        await EnvironmentVariables.BlobStorageConnectionString);
}

static void ConfigureMiddleware(WebApplication app)
{
    app.UseSerilogRequestLogging();
    MigrateDatabase(app);

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseExceptionHandler();

    app.MapControllers();
}

static void MigrateDatabase(IHost app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    //var context = services.GetRequiredService<PaymentContext>();
    //if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
}
