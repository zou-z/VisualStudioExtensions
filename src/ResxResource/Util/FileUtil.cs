using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ResxResource.Util
{
    internal static class FileUtil
    {
        public static Encoding GetFileEncoding(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            var encoding = fileStream != null && fileStream.Length >= 2
                ? GetFileEncoding(fileStream)
                : null;

            return encoding ??= new UTF8Encoding(false, true);
        }

        private static Encoding? GetFileEncoding(FileStream fileStream)
        {
            var encodings = GetEncodingsWhichHasBom()
                .Select(t => new KeyValuePair<Encoding, byte[]>(t, t.GetPreamble()))
                .ToList();

            for (int i = 0; i < fileStream.Length; ++i)
            {
                var character = fileStream.ReadByte();
                for (int j = encodings.Count - 1; j >= 0; --j)
                {
                    if (i >= encodings[j].Value.Length || character != encodings[j].Value[i])
                        encodings.RemoveAt(j);
                    else if (i + 1 == encodings[j].Value.Length)
                        return encodings[j].Key;
                }
            }

            return null;
        }

        private static Encoding[] GetEncodingsWhichHasBom()
        {
            if (encodingsWhichHasBom == null)
            {
                var encodings = new List<Encoding>();
                foreach (var encodingInfo in Encoding.GetEncodings())
                {
                    var encoding = encodingInfo.GetEncoding();
                    if (encoding.GetPreamble().Length > 0)
                    {
                        encodings.Add(encoding);
                    }
                }
                encodingsWhichHasBom = [.. encodings];
            }
            return encodingsWhichHasBom;
        }

        private static Encoding[]? encodingsWhichHasBom = null;
    }
}
