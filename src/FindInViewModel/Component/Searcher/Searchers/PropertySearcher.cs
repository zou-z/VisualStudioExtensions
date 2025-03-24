using FindInViewModel.Model.Search;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FindInViewModel.Component.Searcher.Searchers
{
    internal class PropertySearcher : ISearcher
    {
        public async Task<SearchResult?> SearchAsync(SearchContext context)
        {
            var pattern = $"public (.*?) ({context.BindingText})";

            var fileSources = await context.FindFilesAsyncFunc(
                context.FromProjectName,
                context.TargetFileName,
                context.CancellationToken);
            foreach (var fileSource in fileSources)
            {
                foreach (var filePath in fileSource.FilePaths)
                {
                    var lines = File.ReadAllLines(filePath);
                    for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
                    {
                        var match = Regex.Match(lines[lineIndex], pattern);
                        if (match.Success)
                        {
                            string fromProjectName = fileSource.FromProjectName;
                            string variableTypeName = match.Groups[1].Value;
                            int columnIndex = match.Groups[2].Index;
                            int columnLength = match.Groups[2].Length;
                            var filePosition = new FilePosition(filePath, lineIndex, columnIndex, columnLength);
                            return new SearchResult(fromProjectName, variableTypeName, filePosition);
                        }
                    }
                }
            }

            return null;
        }
    }
}
