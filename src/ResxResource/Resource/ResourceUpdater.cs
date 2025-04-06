using ResxResource.Rule;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ResxResource.Resource
{
    public static class ResourceUpdater
    {
        public static void UpdateResourceDesignerKey(string[] resourceFiles, string originalKey, string newKey)
        {
            var resourceDesignerFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsResourceDesignerFile);
            var neutralResourceFiles = resourceFiles.Where(ResourceNameRule.IsNeutralResourceFile);
            var englishResourceFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsEnglishResourceFile);

            if (!string.IsNullOrEmpty(resourceDesignerFile))
            {
                UpdateResourceDesignerKey(resourceDesignerFile, originalKey, newKey);
            }
            if (neutralResourceFiles != null)
            {
                foreach (var neutralResourceFile in neutralResourceFiles)
                {
                    UpdateResourceKey(neutralResourceFile, originalKey, newKey);
                }
            }
            if (!string.IsNullOrEmpty(englishResourceFile))
            {
                UpdateResourceKey(englishResourceFile, originalKey, newKey);
            }
        }

        public static void UpdateNeutralResourceValue(string[] resourceFiles, string key, string originalValue, string newValue)
        {
            var encodedOriginalValue = HttpUtility.HtmlEncode(originalValue);
            var encodedNewValue = HttpUtility.HtmlEncode(newValue);

            var resourceDesignerFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsResourceDesignerFile);
            var neutralResourceFiles = resourceFiles.Where(ResourceNameRule.IsNeutralResourceFile);

            if (!string.IsNullOrEmpty(resourceDesignerFile))
            {
                UpdateResourceDesignerSummary(resourceDesignerFile, originalValue, newValue);
            }
            if (neutralResourceFiles != null)
            {
                foreach (var neutralResourceFile in neutralResourceFiles)
                {
                    UpdateResourceValue(neutralResourceFile, key, encodedOriginalValue, encodedNewValue);
                }
            }
        }

        public static void UpdateEnglishResourceValue(string[] resourceFiles, string key, string originalValue, string newValue)
        {
            var encodedOriginalValue = HttpUtility.HtmlEncode(originalValue);
            var encodedNewValue = HttpUtility.HtmlEncode(newValue);

            var englishResourceFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsEnglishResourceFile);
            if (!string.IsNullOrEmpty(englishResourceFile))
            {
                UpdateResourceValue(englishResourceFile, key, encodedOriginalValue, encodedNewValue);
            }
        }

        private static void UpdateResourceDesignerSummary(string filePath, string originalSummary, string newSummary)
        {
            originalSummary = $"查找类似 {originalSummary} 的本地化字符串";
            newSummary = $"查找类似 {newSummary} 的本地化字符串";
            ReplaceContent(filePath, originalSummary, newSummary);
        }

        private static void UpdateResourceDesignerKey(string filePath, string originalKey, string newKey)
        {
            var replaceDictionary = new Dictionary<string, string>()
            {
                {
                    $"static string {originalKey} {{",
                    $"static string {newKey} {{"
                },
                {
                    $"return ResourceManager.GetString(\"{originalKey}\", resourceCulture);",
                    $"return ResourceManager.GetString(\"{newKey}\", resourceCulture);"
                }
            };
            ReplaceContents(filePath, replaceDictionary);
        }

        private static void UpdateResourceKey(string filePath, string originalKey, string newKey)
        {
            originalKey = $"<data name=\"{originalKey}\" xml:space=\"preserve\">";
            newKey = $"<data name=\"{newKey}\" xml:space=\"preserve\">";
            ReplaceContent(filePath, originalKey, newKey);
        }

        private static void UpdateResourceValue(string filePath, string key, string originalValue, string newValue)
        {
            originalValue = $"<data name=\"{key}\" xml:space=\"preserve\">\r\n    <value>{originalValue}</value>\r\n  </data>";
            newValue = $"<data name=\"{key}\" xml:space=\"preserve\">\r\n    <value>{newValue}</value>\r\n  </data>";
            ReplaceContent(filePath, originalValue, newValue);
        }

        private static void ReplaceContent(string filePath, string originalContent, string newContent)
        {
            var text = File.ReadAllText(filePath).Replace(originalContent, newContent);
            File.WriteAllText(filePath, text);
        }

        private static void ReplaceContents(string filePath, Dictionary<string, string> replaceDictionary)
        {
            var text = File.ReadAllText(filePath);
            foreach (var keyValuePair in replaceDictionary)
            {
                text = text.Replace(keyValuePair.Key, keyValuePair.Value);
            }
            File.WriteAllText(filePath, text);
        }
    }
}
