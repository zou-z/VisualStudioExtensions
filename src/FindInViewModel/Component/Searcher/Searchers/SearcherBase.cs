using FindInViewModel.Model.Search;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FindInViewModel.Component.Searcher.Searchers
{
    internal abstract class SearcherBase : ISearcher
    {
        public async Task<SearchResult?> SearchAsync(SearchContext context)
        {
            var fileSources = await context.FindFilesAsyncFunc(
                context.FromProjectName,
                context.TargetFileName,
                context.CancellationToken);

            foreach (var fileSource in fileSources)
            {
                foreach (var filePath in fileSource.FilePaths)
                {
                    // 匹配目标文件
                    var result = MatchFile(
                        fileSource.FromProjectName,
                        filePath,
                        context.BindingText,
                        out string matchedText);
                    if (result != null)
                    {
                        return result;
                    }

                    // 查找基类文件
                    var viewModelTypeName = Path.GetFileNameWithoutExtension(context.TargetFileName);
                    var baseClassName = GetBaseClassName(matchedText, viewModelTypeName);
                    if (!string.IsNullOrEmpty(baseClassName))
                    {
                        result = await SearchAsync(new SearchContext(
                            fileSource.FromProjectName,
                            $"{baseClassName}.cs",
                            context.BindingText,
                            context.FindFilesAsyncFunc,
                            context.CancellationToken));
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        protected abstract SearchResult? MatchLine(
            string fromProjectName,
            string filePath,
            string originalText,
            string bindingText,
            int lineIndex);

        private SearchResult? MatchFile(
            string fromProjectName,
            string filePath,
            string bindingText,
            out string matchedText)
        {
            matchedText = string.Empty;

            var lines = File.ReadLines(filePath);
            using var enumerator = lines.GetEnumerator();
            for (int lineIndex = 0; enumerator.MoveNext(); ++lineIndex)
            {
                var result = MatchLine(
                    fromProjectName,
                    filePath,
                    enumerator.Current,
                    bindingText,
                    lineIndex);
                if (result != null)
                {
                    return result;
                }
                matchedText += enumerator.Current;
            }

            return null;
        }

        private static string GetBaseClassName(string text, string className)
        {
            text = text.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            var pattern = $"[class|record]{className}.*?:(.*?)[,|{{|\\<]";
            var match = Regex.Match(text, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
