using AICommunicationService.BLL.Extensions;
using AICommunicationService.Common;
using AICommunicationService.Common.Constants;
using AICommunicationService.Common.Enums;
using AICommunicationService.DAL.Context.AiCommunication;
using AICommunicationService.DAL.Extensions;
using AICommunicationService.Middlewares;
using AICommunicationService.RAG.Context.Vector;
using AICommunicationService.RAG.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

await AddServicesAsync(builder);

var app = builder.Build();

AddMiddleware(app);

app.MapControllers();

MigrateDatabase(app);
MigrateRAG(app);

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
                .AllowCredentials()
                .WithExposedHeaders("*");
        });
    });

    builder.Services.AddSignalR();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(PolicyConstants.REQUIRE_BASIC_ACCOUNT, policy =>
            policy.RequireClaim(JwtOptions.ACCOUNT_PLAN, AccountPlan.Basic.ToString()));
    });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(async options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JwtOptions.ISSUER,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = JwtOptions.GetSymmetricSecurityKey(await EnvironmentVariables.GetJwtSecretKeyAsync),
                ValidateIssuerSigningKey = true
            };
        });

    builder.Services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ellogy. AI communication service API", Version = "v1" });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Just paste token value in field.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        options.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddHttpClient("AzureAiRequest",
        async client =>
        {
            client.DefaultRequestHeaders.Add("api-key", await EnvironmentVariables.GetOpenAiKeyAsync);
            client.Timeout = TimeSpan.FromMinutes(3);
        });

    builder.Services.AddHttpClient("GroqAiRequest",
        async client =>
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await EnvironmentVariables.GetGroqKeyAsync}");
        });

    builder.Services.AddHealthChecks();
    builder.Services.AddDataLayer(
        await EnvironmentVariables.GetConnectionStringAsync,
        await EnvironmentVariables.GetConnectionStringPaymentAsync);
    builder.Services.AddRAGDataLayer(await EnvironmentVariables.GetConnectionStringVectorAsync);
    builder.Services.AddBusinessLayer(await EnvironmentVariables.GetBlobStorageConnectionStringAsync);
    builder.Services.AddMapping();
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

    var context = services.GetRequiredService<AICommunicationContext>();
    if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
}

static void MigrateRAG(IHost app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<VectorContext>();
    if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
}