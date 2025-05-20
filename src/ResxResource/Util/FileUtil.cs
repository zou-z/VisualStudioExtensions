using System.IO;

namespace ResxResource.Util
{
    internal static class FileUtil
    {
        public static bool IsUtf8WithBom(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            return stream.ReadByte() == 0xEF && stream.ReadByte() == 0xBB && stream.ReadByte() == 0xBF;
        }

        public static string GetBtf8BomString()
            => $"{(char)0xEF}{(char)0xBB}{(char)0xBF}";
    }
}
