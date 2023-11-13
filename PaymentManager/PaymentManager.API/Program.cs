using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaymentManager.API.Middlewares;
using PaymentManager.BLL.Extensions;
using PaymentManager.BLL.Hubs;
using PaymentManager.Common;
using PaymentManager.Common.Options;
using PaymentManager.DAL.Context.PaymentContext;
using PaymentManager.DAL.Extensions;
using Stripe;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

AddServices(builder);

StripeConfiguration.ApiKey = EnvironmentVariables.SecretKey;

var app = builder.Build();
MigrateDatabase(app);
AddMiddleware(app);

app.MapControllers();

app.Run();

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddCors();
    builder.Services.AddSignalR();
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JwtOptions.Issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = JwtOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
            };
        });

    builder.Services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "Ellogy. Payment Manager service API",
            Version = "v1",
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Just paste token value in field.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new()
        {
            {
                new()
                    {
                        Reference = new()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                new List<string>()
            }
        });
    });

    builder.Services.AddBusinessLayer();
    builder.Services.AddDataLayer(EnvironmentVariables.ConnectionString, EnvironmentVariables.ConnectionStringPayment);
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

    app.UseCors(builder =>
    {
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
        builder.SetIsOriginAllowed(origin => true);
        builder.AllowCredentials();
    });
    app.UseWebSockets();

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHub<PaymentHub>("/paymentHub");
    app.UseMiddleware<ExceptionHandlerMiddleware>();
}
