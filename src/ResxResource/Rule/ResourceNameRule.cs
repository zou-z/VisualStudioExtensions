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
            return !IsResourceDesignerFile(fileName) && !IsEnglishResourceFile(fileName);
        }

        public static bool IsEnglishResourceFile(string fileName)
        {
            return fileName.EndsWith("en-US.resx");
        }
    }
}
