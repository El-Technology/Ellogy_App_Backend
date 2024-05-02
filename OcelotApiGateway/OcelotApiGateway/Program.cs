using Newtonsoft.Json.Linq;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var ocelotConf = Environment.GetEnvironmentVariable("Ocelot_conf") ?? "local";
var configurationBuilder = new ConfigurationBuilder();

var jsonString = File.ReadAllText($"ocelot.{ocelotConf}.json");
var updatedJsonString = UpdateHost(jsonString);

configurationBuilder.AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(updatedJsonString)));

builder.Services.AddOcelot(configurationBuilder.Build()).AddPolly();
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

static string UpdateHost(string jsonString)
{
    var jsonObject = JObject.Parse(jsonString);
    var routes = jsonObject["Routes"] as JArray
        ?? throw new InvalidCastException();

    foreach (var route in routes)
    {
        var downstreamHostAndPorts = route["DownstreamHostAndPorts"] as JArray
            ?? throw new InvalidCastException();

        foreach (var hostAndPort in downstreamHostAndPorts)
            hostAndPort["Host"] = Environment.GetEnvironmentVariable("SSH_HOST");
    }

    return jsonObject.ToString();
}