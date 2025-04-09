using System.IO;

namespace ResxResource.Rule
{
    internal static class ResourceNameRule
    {
        public static bool IsResourceDesignerFile(string fileName)
        {
            return fileName.EndsWith(".Designer.cs");
        }

        public static bool IsNeutralResourceFile(string fileName)
        {
            return fileName.EndsWith("zh-CN.resx") || Path.GetFileName(fileName) == "Resources.resx";
        }

        public static bool IsEnglishResourceFile(string fileName)
        {
            return fileName.EndsWith("en-US.resx");
        }
    }
}
