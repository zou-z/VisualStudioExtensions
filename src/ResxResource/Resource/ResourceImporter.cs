using ResxResource.Model;
using ResxResource.Rule;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ResxResource.Resource
{
    public static class ResourceImporter
    {
        public static bool GetImportResourcesFromText(string text, out ResourceItem[] resources)
        {
            var lines = text.Split("\r\n".ToCharArray()).Select(t => t.Trim()).Where(t => t != string.Empty).ToList();
            if (lines.Count % 3 != 0)
            {
                resources = [];
                return false;
            }

            var list = new List<ResourceItem>(lines.Count / 3);
            for (int i = 0; i < lines.Count; i += 3)
            {
                list.Add(new ResourceItem(lines[i], lines[i + 1], lines[i + 2]));
            }
            resources = [.. list];
            return true;
        }

        public static bool Import(string[] resourceFiles, ResourceItem[] resources, string modifier)
        {
            var resourceDesignerFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsResourceDesignerFile);
            var neutralResourceFiles = resourceFiles.Where(ResourceNameRule.IsNeutralResourceFile);
            var englishResourceFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsEnglishResourceFile);

            if (!string.IsNullOrEmpty(resourceDesignerFile))
            {
                if (!ImportResourceDesigner(resourceDesignerFile, resources, modifier))
                {
                    return false;
                }
            }
            if (neutralResourceFiles != null)
            {
                foreach (var neutralResourceFile in neutralResourceFiles)
                {
                    if (!ImportResource(neutralResourceFile, resources, true))
                    {
                        return false;
                    }
                }
            }
            if (!string.IsNullOrEmpty(englishResourceFile))
            {
                if (!ImportResource(englishResourceFile, resources, false))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ImportResourceDesigner(string filePath, ResourceItem[] resources, string modifier)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            if (!SeekToLastPosition(fileStream, '}', 3))
            {
                return false;
            }

            var indentCount = GetCurrentLineIndentCount(fileStream);
            var indentString = new string(' ', indentCount);

            foreach (var resource in resources)
            {
                var lines = GenerateResourcesDesignerLines(resource.Key, resource.NeutralText, modifier);

                InsertTextToFile(fileStream, "\r\n");
                foreach (var line in lines)
                {
                    var text = indentString + line;
                    InsertTextToFile(fileStream, text);
                }
            }

            return true;
        }

        private static bool ImportResource(string filePath, ResourceItem[] resources, bool isNeutralText)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            if (!SeekToLastPosition(fileStream, '>', 2))
            {
                return false;
            }

            var indentCount = GetCurrentLineIndentCount(fileStream);
            var indentString = new string(' ', indentCount);

            foreach (var resource in resources)
            {
                var lines = GenerateResourcesLines(resource.Key, isNeutralText ? resource.NeutralText : resource.EnglishText);

                InsertTextToFile(fileStream, "\r\n");
                foreach (var line in lines)
                {
                    var text = indentString + line;
                    InsertTextToFile(fileStream, text);
                }
            }

            return true;
        }

        private static bool SeekToLastPosition(FileStream fileStream, char character, int matchCount)
        {
            int characterCount = 0;
            int characterAscii = character;

            fileStream.Position = fileStream.Length - 1;
            while (fileStream.Position >= 0)
            {
                if (fileStream.ReadByte() == characterAscii)
                {
                    ++characterCount;
                }
                if (characterCount == matchCount)
                {
                    return true;
                }
                fileStream.Position -= 2;
            }
            return false;
        }

        private static int GetCurrentLineIndentCount(FileStream fileStream)
        {
            var position = fileStream.Position;

            int lineBreakAscii = '\n';
            while (fileStream.ReadByte() != lineBreakAscii && fileStream.Position > 0)
            {
                fileStream.Position -= 2;
            }

            int indentCount = 0;
            int spaceAscii = ' ';
            while (fileStream.Position < fileStream.Length &&
                fileStream.ReadByte() == spaceAscii)
            {
                ++indentCount;
            }

            fileStream.Position = position;
            return indentCount;
        }

        private static void InsertTextToFile(FileStream fileStream, string text)
        {
            var position = fileStream.Position;
            var fileTailBuffer = new byte[fileStream.Length - fileStream.Position];
            fileStream.Read(fileTailBuffer, 0, fileTailBuffer.Length);
            fileStream.Position = position;

            var buffer = Encoding.UTF8.GetBytes(text);
            fileStream.Write(buffer, 0, buffer.Length);

            position = fileStream.Position;
            fileStream.Write(fileTailBuffer, 0, fileTailBuffer.Length);
            fileStream.Position = position;
        }

        private static string[] GenerateResourcesDesignerLines(string key, string neutralText, string modifier)
        {
            return
            [
                "\r\n",
                "/// <summary>\r\n",
                $"///   查找类似 {neutralText} 的本地化字符串。\r\n",
                "/// </summary>\r\n",
                $"{modifier} static string {key} {{\r\n",
                "    get {\r\n",
                $"        return ResourceManager.GetString(\"{key}\", resourceCulture);\r\n",
                "    }\r\n",
                "}",
            ];
        }

        private static string[] GenerateResourcesLines(string key, string value)
        {
            return
            [
                $"<data name=\"{key}\" xml:space=\"preserve\">\r\n",
                $"  <value>{HttpUtility.HtmlDecode(value)}</value>\r\n",
                "</data>",
            ];
        }
    }
}
