using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
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
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new() { Title = "Ellogy. Tickets Manager service API", Version = "v1" });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Just paste token value in field.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        options.AddSecurityRequirement(new()
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

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        options.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddHealthChecks();

    builder.Services.AddDataLayer(EnvironmentVariables.ConnectionString);
    builder.Services.AddBusinessLayer();
    builder.Services.AddMapping();
}

static void AddMiddleware(WebApplication app)
{
    app.UseCors();

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
