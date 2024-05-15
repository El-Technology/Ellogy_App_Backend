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

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();
app.MapControllers();

app.MapPost("dev/Communication/getStreamResponse", async context =>
{
    var response = await GetStreamResponseAsync(context);

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
        Console.WriteLine(await reader.ReadLineAsync());
        await context.Response.WriteAsync($"{await reader.ReadLineAsync()}\n");
        await context.Response.Body.FlushAsync();
    }
});

app.UseCors();
app.UseWebSockets();
await app.UseOcelot();

app.Run();

static async Task<HttpResponseMessage> GetStreamResponseAsync(HttpContext context)
{
    context.Response.Headers.Add("Content-Type", "text/event-stream");

    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
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

    var message = new HttpRequestMessage
    {
        RequestUri = new Uri(
            $"http://{Environment.GetEnvironmentVariable("SSH_HOST")}:53053/api/Communication/getStreamResponse"),
        Method = HttpMethod.Post,
        Content = content
    };

    return await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
}

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