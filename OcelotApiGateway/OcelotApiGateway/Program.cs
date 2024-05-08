using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var ocelotConf = Environment.GetEnvironmentVariable("Ocelot_conf") ?? "local";
var configurationBuilder = new ConfigurationBuilder();

configurationBuilder.AddJsonFile($"ocelot.{ocelotConf}.json", optional: false, reloadOnChange: true);

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

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();
app.MapControllers();

app.MapPost("dev/Communication/getStreamResponse", async context =>
{
    context.Response.Headers.Add("Content-Type", "text/event-stream");

    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var jsonRequest = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    using var httpClient = new HttpClient();

    foreach (var header in context.Request.Headers)
    {
        if (header.Key != "Content-Type"
            && header.Key != "Host"
            && header.Key != "Connection"
            && header.Key != "Content-Length")
        {
            httpClient.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
        }
    }

    using var response = await httpClient.PostAsync(
        "http://20.21.129.55:53053/api/Communication/getStreamResponse", content);
    using var stream = await response.Content.ReadAsStreamAsync();

    if (!response.IsSuccessStatusCode)
    {
        context.Response.StatusCode = (int)response.StatusCode;
        await stream.CopyToAsync(context.Response.Body);
        return;
    }

    using var reader = new StreamReader(stream);
    while (!reader.EndOfStream)
    {
        var line = await reader.ReadLineAsync();
        await context.Response.WriteAsync(line ?? string.Empty);
    }
});

app.UseCors();
app.UseWebSockets();
await app.UseOcelot();

app.Run();
