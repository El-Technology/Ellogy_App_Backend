using AICommunicationService.Api.Middlewares;
using AICommunicationService.BLL.Extensions;
using AICommunicationService.BLL.Hubs;
using AICommunicationService.Common;
using AICommunicationService.DAL.Context;
using AICommunicationService.DAL.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using TicketsManager.Common;

var builder = WebApplication.CreateBuilder(args);

AddServices(builder);

var app = builder.Build();

AddMiddleware(app);

app.MapControllers();
MigrateDatabase(app);
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
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new() { Title = "Ellogy. AI communication service API", Version = "v1" });

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
    app.UseHealthChecks("/health");
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapHub<StreamAiHub>("/streamHub");
    app.UseMiddleware<ExceptionHandlerMiddleware>();
}
static void MigrateDatabase(IHost app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AICommunicationContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}