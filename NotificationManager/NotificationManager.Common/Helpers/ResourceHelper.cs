namespace NotificationManager.Common.Helpers;
public static class ResourceHelper
{
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
