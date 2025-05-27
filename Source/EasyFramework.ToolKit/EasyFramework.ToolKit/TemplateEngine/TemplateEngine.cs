using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EasyFramework.Core;

namespace EasyFramework.ToolKit
{
    public static class TemplateEngine
    {
        private static string NormalizeKey(string key)
        {
            // Handles snake_case, potential kebab-case in templates, and general case insensitivity
            // by lowercasing and removing underscores and hyphens.
            return key.Replace("_", "").Replace("-", "").ToLowerInvariant();
        }

        public static string Render(string template, object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data object cannot be null for template rendering.");
            }

            var dict = data.GetType()
                .GetMembers(BindingFlagsHelper.AllInstance())
                .Where(member => member is FieldInfo || member is PropertyInfo)
                .ToDictionary(member => member.Name, member => member.GetMemberValue(data));

            // Regex to find placeholders like {{ key }}, {{key}}, {{ key_name }}
            // It allows letters, numbers, and underscores in key names, surrounded by optional whitespace.
            string pattern = @"{{\s*(?<keyName>[a-zA-Z0-9_]+)\s*}}";

            string result = Regex.Replace(template, pattern, match =>
            {
                string keyFromTemplate = match.Groups["keyName"].Value;
                string normalizedKeyFromTemplate = NormalizeKey(keyFromTemplate);

                foreach (var kvp in dict)
                {
                    string normalizedDictKey = NormalizeKey(kvp.Key);
                    if (normalizedDictKey == normalizedKeyFromTemplate)
                    {
                        return kvp.Value?.ToString() ?? string.Empty;
                    }
                }
                // If no match is found, leave the tag as is in the template.
                // This helps in debugging (e.g. misspelled variable names).
                return match.Value;
            });

            return result;
        }
    }
}
