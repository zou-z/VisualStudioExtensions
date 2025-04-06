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
            return fileName.EndsWith(".resx") || fileName.EndsWith("zh-CN.resx");
        }

        public static bool IsEnglishResourceFile(string fileName)
        {
            return fileName.EndsWith("en-US.resx");
        }
    }
}
