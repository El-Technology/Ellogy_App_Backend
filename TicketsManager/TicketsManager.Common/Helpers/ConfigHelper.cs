using Microsoft.Extensions.Configuration;
using TicketsManager.Common.Constants;

namespace TicketsManager.Common.Helpers;
/// <summary>
/// ConfigHelper
/// </summary>
public class ConfigHelper
{
    private static ConfigHelper? _appSettings;

    /// <summary>
    /// AppSetting value
    /// </summary>
    public string _appSettingValue { get; set; }

    /// <summary>
    /// Get the AppSetting value
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    public static string AppSetting(string Key)
    {
        _appSettings = GetCurrentSettings(Key);
        return _appSettings._appSettingValue;
    }

    /// <summary>
    /// Constructor for the ConfigHelper
    /// </summary>
    /// <param name="config"></param>
    /// <param name="Key"></param>
    public ConfigHelper(IConfiguration config, string Key)
    {
        _appSettingValue = config.GetValue<string>(Key);
    }

    /// <summary>
    /// Get the current settings
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    public static ConfigHelper GetCurrentSettings(string Key)
    {
        var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(ConfigConstants.ConfigFileName, optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

        var configuration = builder.Build();

        var settings = new ConfigHelper(configuration.GetSection(ConfigConstants.ConfigSectionName), Key);

        return settings;
    }
}
