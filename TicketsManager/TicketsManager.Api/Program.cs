using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;
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
        c.SwaggerDoc("v1", new() { Title = "Ellogy API", Version = "v1" });

        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "Standard Authorization header using Bearer scheme, e.g. \"bearer {token}\"",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        c.OperationFilter<SecurityRequirementsOperationFilter>();
    });

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

    app.UseRouting();
    app.UseAuthentication();
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
