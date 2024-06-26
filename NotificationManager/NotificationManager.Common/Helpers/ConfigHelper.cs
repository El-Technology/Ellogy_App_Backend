using Microsoft.Extensions.Configuration;
using NotificationManager.Common.Constants;

namespace NotificationManager.Common.Helpers;

/// <summary>
/// ConfigHelper
/// </summary>
/// <remarks>
/// Constructor for the ConfigHelper
/// </remarks>
/// <param name="config"></param>
/// <param name="Key"></param>
public class ConfigHelper(IConfiguration config, string Key)
{
    private static ConfigHelper? _appSettings;

    /// <summary>
    /// AppSetting value
    /// </summary>
    public string? AppSettingValue { get; set; } = config.GetValue<string>(Key);

    /// <summary>
    /// Get the AppSetting value
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    public static string? AppSetting(string Key)
    {
        _appSettings = GetCurrentSettings(Key);
        return _appSettings.AppSettingValue;
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
