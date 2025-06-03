using ResxResource.Rule;
using ResxResource.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ResxResource.Resource
{
    public static class ResourceUpdater
    {
        public static void UpdateKey(string[] resourceFiles, string oldKey, string newKey)
        {
            var resourceDesignerFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsResourceDesignerFile);
            var neutralResourceFiles = resourceFiles.Where(ResourceNameRule.IsNeutralResourceFile);
            var englishResourceFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsEnglishResourceFile);

            if (!string.IsNullOrEmpty(resourceDesignerFile))
            {
                UpdateResourceDesignerKey(resourceDesignerFile, oldKey, newKey);
            }
            if (neutralResourceFiles != null)
            {
                foreach (var neutralResourceFile in neutralResourceFiles)
                {
                    UpdateResourceKey(neutralResourceFile, oldKey, newKey);
                }
            }
            if (!string.IsNullOrEmpty(englishResourceFile))
            {
                UpdateResourceKey(englishResourceFile, oldKey, newKey);
            }
        }

        public static void UpdateNeutralText(string[] resourceFiles, string key, string oldNeutralText, string newNeutralText)
        {
            var encodedOriginalValue = XmlEncode(oldNeutralText);
            var encodedNewValue = XmlEncode(newNeutralText);

            var resourceDesignerFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsResourceDesignerFile);
            var neutralResourceFiles = resourceFiles.Where(ResourceNameRule.IsNeutralResourceFile);

            if (!string.IsNullOrEmpty(resourceDesignerFile))
            {
                UpdateResourceDesignerSummary(resourceDesignerFile, oldNeutralText, newNeutralText);
            }
            if (neutralResourceFiles != null)
            {
                foreach (var neutralResourceFile in neutralResourceFiles)
                {
                    UpdateResourceValue(neutralResourceFile, key, encodedOriginalValue, encodedNewValue);
                }
            }
        }

        public static void UpdateEnglishText(string[] resourceFiles, string key, string oldEnglishText, string newEnglishText)
        {
            var encodedOriginalValue = XmlEncode(oldEnglishText);
            var encodedNewValue = XmlEncode(newEnglishText);

            var englishResourceFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsEnglishResourceFile);
            if (!string.IsNullOrEmpty(englishResourceFile))
            {
                UpdateResourceValue(englishResourceFile, key, encodedOriginalValue, encodedNewValue);
            }
        }

        private static void UpdateResourceDesignerSummary(string filePath, string oldSummary, string newSummary)
        {
            oldSummary = $"查找类似 {oldSummary} 的本地化字符串";
            newSummary = $"查找类似 {newSummary} 的本地化字符串";
            ReplaceContent(filePath, oldSummary, newSummary);
        }

        private static void UpdateResourceDesignerKey(string filePath, string oldKey, string newKey)
        {
            var replaceDictionary = new Dictionary<string, string>()
            {
                {
                    $"static string {oldKey} {{",
                    $"static string {newKey} {{"
                },
                {
                    $"return ResourceManager.GetString(\"{oldKey}\", resourceCulture);",
                    $"return ResourceManager.GetString(\"{newKey}\", resourceCulture);"
                }
            };
            ReplaceContents(filePath, replaceDictionary);
        }

        private static void UpdateResourceKey(string filePath, string oldKey, string newKey)
        {
            oldKey = $"<data name=\"{oldKey}\" xml:space=\"preserve\">";
            newKey = $"<data name=\"{newKey}\" xml:space=\"preserve\">";
            ReplaceContent(filePath, oldKey, newKey);
        }

        private static void UpdateResourceValue(string filePath, string key, string oldValue, string newValue)
        {
            oldValue = $"<data name=\"{key}\" xml:space=\"preserve\">\r\n    <value>{oldValue}</value>\r\n  </data>";
            newValue = $"<data name=\"{key}\" xml:space=\"preserve\">\r\n    <value>{newValue}</value>\r\n  </data>";
            ReplaceContent(filePath, oldValue, newValue);
        }

        private static void ReplaceContent(string filePath, string oldContent, string newContent)
        {
            var encoding = FileUtil.GetFileEncoding(filePath);
            var text = File.ReadAllText(filePath).Replace(oldContent, newContent);

            File.WriteAllText(filePath, text, encoding);
        }

        private static void ReplaceContents(string filePath, Dictionary<string, string> replaceDictionary)
        {
            var encoding = FileUtil.GetFileEncoding(filePath);
            var text = File.ReadAllText(filePath);
            foreach (var keyValuePair in replaceDictionary)
            {
                text = text.Replace(keyValuePair.Key, keyValuePair.Value);
            }

            File.WriteAllText(filePath, text, encoding);
        }

        private static string XmlEncode(string text)
            => text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
