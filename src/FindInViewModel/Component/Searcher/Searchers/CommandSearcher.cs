using FindInViewModel.Extension;
using FindInViewModel.Model.Search;
using System.Text.RegularExpressions;

namespace FindInViewModel.Component.Searcher.Searchers
{
    internal class CommandSearcher : SearcherBase
    {
        protected override void OnSearchStart(SearchContext context)
        {
            var methodName = context.BindingText.GetBindingMethodName();
            var pattern = $"(void|Task|Task<.+>) ({methodName})\\(";
            regex = new Regex(pattern);
        }

        protected override void OnSearchEnd()
        {
            regex = null;
        }

        protected override SearchResult? MatchLine(
            string fromProjectName,
            string filePath,
            string originalText,
            int lineIndex)
        {
            var match = regex?.Match(originalText);
            if (match?.Success == true)
            {
                int columnIndex = match.Groups[2].Index;
                int columnLength = match.Groups[2].Length;
                var filePosition = new FilePosition(filePath, lineIndex, columnIndex, columnLength);
                return new SearchResult(fromProjectName, string.Empty, filePosition);
            }
            return null;
        }

        private Regex? regex = null;
    }
}
