using FindInViewModel.Extension;
using FindInViewModel.Model.Search;
using System.Text.RegularExpressions;

namespace FindInViewModel.Component.Searcher.Searchers
{
    internal class CommandSearcher : SearcherBase
    {
        protected override SearchResult? MatchLine(
            string fromProjectName,
            string filePath,
            string originalText,
            string bindingText,
            int lineIndex)
        {
            var methodName = bindingText.GetBindingMethodName();
            var pattern = $"(void|Task|Task<.+>) ({methodName})\\(";
            var match = Regex.Match(originalText, pattern);
            if (match.Success)
            {
                int columnIndex = match.Groups[2].Index;
                int columnLength = match.Groups[2].Length;
                var filePosition = new FilePosition(filePath, lineIndex, columnIndex, columnLength);
                return new SearchResult(fromProjectName, string.Empty, filePosition);
            }
            return null;
        }
    }
}
