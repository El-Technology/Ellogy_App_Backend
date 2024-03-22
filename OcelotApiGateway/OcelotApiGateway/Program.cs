using Newtonsoft.Json.Linq;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services);

        var app = builder.Build();

        Configure(app);

        app.Run();
    }

    static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.SetIsOriginAllowed(origin => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        services.AddOcelot();
    }

    static void Configure(WebApplication app)
    {
        var ocelotConf = Environment.GetEnvironmentVariable("Ocelot_conf") ?? "local";
        var configurationBuilder = new ConfigurationBuilder();

        var jsonString = File.ReadAllText($"ocelot.{ocelotConf}.json");
        var updatedJsonString = UpdateHost(jsonString);
        File.WriteAllText($"ocelot.{ocelotConf}.json", updatedJsonString);

        configurationBuilder.AddJsonFile($"ocelot.{ocelotConf}.json", optional: false, reloadOnChange: true);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseWebSockets();
        app.UseOcelot();
        app.MapControllers();
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
}
