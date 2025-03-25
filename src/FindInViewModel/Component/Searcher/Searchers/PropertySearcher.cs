using FindInViewModel.Model.Search;
using System.Text.RegularExpressions;

namespace FindInViewModel.Component.Searcher.Searchers
{
    internal class PropertySearcher : SearcherBase
    {
        protected override SearchResult? MatchLine(
            string fromProjectName,
            string filePath,
            string originalText,
            string bindingText,
            int lineIndex)
        {
            var pattern = $"public (.*?) ({bindingText})";
            var match = Regex.Match(originalText, pattern);
            if (match.Success)
            {
                string variableTypeName = match.Groups[1].Value;
                int columnIndex = match.Groups[2].Index;
                int columnLength = match.Groups[2].Length;
                var filePosition = new FilePosition(filePath, lineIndex, columnIndex, columnLength);
                return new SearchResult(fromProjectName, variableTypeName, filePosition);
            }
            return null;
        }
    }
}
