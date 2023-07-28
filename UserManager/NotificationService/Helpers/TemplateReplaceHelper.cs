using System.Collections.Generic;
using System.Text;

namespace NotificationService.Helpers
{
    public static class TemplateReplaceHelper
    {
        public static string Replace(string template, Dictionary<string, string> metaData)
        {
            var replacedTemplate = new StringBuilder(template);

            foreach (var keyValue in metaData)
                if (template.Contains(keyValue.Key))
                    replacedTemplate.Replace(keyValue.Key, keyValue.Value);

            return replacedTemplate.ToString();
        }
    }
}
