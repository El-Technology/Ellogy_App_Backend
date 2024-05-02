using System.Text.Json;

namespace AICommunicationService.Common.Helpers;
public static class DbSeedHelper
{
    public static T? ConvertJsonToList<T>(string path, string projectName)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
        var jsonText = File.ReadAllText(fullPath.Replace(".Api", projectName));

        return JsonSerializer.Deserialize<T>(jsonText);
    }
}