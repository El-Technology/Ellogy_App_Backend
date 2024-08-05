namespace NotificationManager.Common.Helpers;

/// <summary>
/// Helper class for resources
/// </summary>
public static class ResourceHelper
{
    /// <summary>
    /// Resource helper for getting html templates from project resources
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static string GetHtmlTemplate(string fileName)
    {
        var assembly = typeof(ResourceHelper).Assembly;
        var resourceName = $"NotificationManager.Common.HtmlTemplates.{fileName}";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new FileNotFoundException("Resource not found: " + resourceName);

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
