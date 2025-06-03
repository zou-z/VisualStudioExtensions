using ResxResource.Rule;
using ResxResource.Util;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ResxResource.Resource
{
    public static class ResourceDeleter
    {
        public static bool Delete(string[] resourceFiles, string[] resourceKeys)
        {
            var resourceDesignerFile = resourceFiles.FirstOrDefault(ResourceNameRule.IsResourceDesignerFile);
            var otherResourceFiles = resourceFiles.Where(t => t != resourceDesignerFile);

            var resourceDesignerRegex = new Regex("[public|internal] static string (.*?) {");
            var resourceRegex = new Regex("<data name=\"(.*?)\" xml:space=\"preserve\">");

            if (resourceDesignerFile != null)
            {
                if (!DeleteContent(resourceDesignerFile, resourceKeys, resourceDesignerRegex, 1, 4, 9))
                {
                    return false;
                }
            }
            if (otherResourceFiles.Any(t => !DeleteContent(t, resourceKeys, resourceRegex, 1, 0, 3)))
            {
                return false;
            }

            return true;
        }

        private static bool DeleteContent(
            string filePath,
            string[] resourceKeys,
            Regex regex,
            int groupIndex,
            int frontLineCount,
            int behindLineCount)
        {
            var resourceKeyList = resourceKeys.ToList();

            var lines = File.ReadAllLines(filePath).ToList();
            for (int i = 0; i < lines.Count && resourceKeyList.Count > 0;)
            {
                var result = regex.Match(lines[i]);
                if (result.Success)
                {
                    int index = resourceKeyList.FindIndex(t => t == result.Groups[groupIndex].Value);
                    if (index != -1)
                    {
                        resourceKeyList.RemoveAt(index);
                        lines.RemoveRange(i - frontLineCount, behindLineCount);
                    }
                    else
                    {
                        i += behindLineCount;
                    }
                }
                else
                {
                    ++i;
                }
            }

            if (resourceKeyList.Count == 0)
            {
                var encoding = FileUtil.GetFileEncoding(filePath);
                File.WriteAllLines(filePath, lines, encoding);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
