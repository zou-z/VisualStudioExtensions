using ResxResource.Model;
using ResxResource.Rule;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ResxResource.Resource
{
    public static class ResourceReader
    {
        public static ResourceItem[] Read(string[] resourceFiles, out string modifier)
        {
            var resourceDesignerFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsResourceDesignerFile);
            var neutralResourceFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsNeutralResourceFile);
            var englishResourceFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsEnglishResourceFile);
            if (string.IsNullOrEmpty(resourceDesignerFile))
            {
                modifier = string.Empty;
                return [];
            }

            var keys = GetKeysFromResourceDesigner(resourceDesignerFile, out modifier);
            var neutralValues = GetValuesFromResource(neutralResourceFile);
            var englishValues = GetValuesFromResource(englishResourceFile);

            return [.. keys.Select(key => new ResourceItem(
                key,
                neutralValues.ContainsKey(key) ? HttpUtility.HtmlDecode(neutralValues[key]) : string.Empty,
                englishValues.ContainsKey(key) ? HttpUtility.HtmlDecode(englishValues[key]) : string.Empty))];
        }

        private static string[] GetKeysFromResourceDesigner(string filePath, out string modifier)
        {
            modifier = string.Empty;
            var lines = File.ReadLines(filePath);
            using var enumerator = lines.GetEnumerator();

            var list = new List<string>();
            var regex = new Regex("(public|internal) static string (.*?) {");
            while (enumerator.MoveNext())
            {
                var result = regex.Match(enumerator.Current);
                if (result.Success)
                {
                    modifier = modifier == string.Empty ? result.Groups[1].Value : modifier;
                    list.Add(result.Groups[2].Value);
                }
            }
            return [.. list];
        }

        private static Dictionary<string, string> GetValuesFromResource(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return [];
            }

            var text = File.ReadAllText(filePath).Replace(" ", "").Replace("\r", "").Replace("\n", "");
            var pattern = "<dataname=\"([^<>\"]*?)\"xml:space=\"preserve\"><value>([^<>]*?)</value></data>";
            var result = Regex.Matches(text, pattern);
            return result.Cast<Match>().ToDictionary(
                match => match.Groups[1].Value,
                match => match.Groups[2].Value);
        }
    }
}
