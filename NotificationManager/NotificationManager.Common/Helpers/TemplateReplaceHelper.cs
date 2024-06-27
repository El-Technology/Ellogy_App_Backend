using System.Text;

namespace NotificationManager.Common.Helpers;

/// <summary>
/// Helper class for replacing template values
/// </summary>
public static class TemplateReplaceHelper
{
    /// <summary>
    /// Replace template values with metadata
    /// </summary>
    /// <param name="template"></param>
    /// <param name="metaData"></param>
    /// <returns></returns>
    public static string Replace(string template, Dictionary<string, string> metaData)
    {
        var replacedTemplate = new StringBuilder(template);

        foreach (var keyValue in metaData)
            if (template.Contains(keyValue.Key))
                replacedTemplate.Replace(keyValue.Key, keyValue.Value);

        return replacedTemplate.ToString();
    }
}
