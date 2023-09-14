using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var ocelotConf = Environment.GetEnvironmentVariable("Ocelot_conf") ?? "local";
var configurationBuilder = new ConfigurationBuilder();

if (ocelotConf.Equals("azure"))
{
    configurationBuilder.AddJsonFile($"ocelot.azure.json", optional: false, reloadOnChange: true);
    configurationBuilder.AddJsonFile($"ocelot.azureAdmin.json", optional: false, reloadOnChange: true);
}
else
    configurationBuilder.AddJsonFile($"ocelot.local.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(configurationBuilder.Build());
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseCors();
app.UseWebSockets();
await app.UseOcelot();

app.Run();
