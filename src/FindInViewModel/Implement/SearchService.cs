using FindInViewModel.Component.Searcher;
using FindInViewModel.Extension;
using FindInViewModel.Model;
using FindInViewModel.Model.Search;
using FindInViewModel.Service;
using System.Linq;

namespace FindInViewModel.Implement
{
    internal class SearchService : ISearchService
    {
        public FilePosition? FindAsync(string fromProjectName, string fromFileName, string[] bindings, FindFilesFunc findFilesFunc)
        {
            bindings = TrimBindingsBehindCommandBinding(bindings);

            SearchResult? result = new(fromProjectName, fromFileName.GetViewModelName(), FilePosition.Empty);
            for (int i = 0; i < bindings.Length; ++i)
            {
                var targetFileName = $"{result!.VariableTypeName}.cs";
                fromProjectName = result.FromProjectName;
                do
                {
                    result = SearcherFactory.Create(bindings[i])
                        .Search(new SearchContext(fromProjectName, targetFileName, bindings[i], findFilesFunc));
                }
                while (result == null && ++i < bindings.Length);
            }

            return result?.FilePosition;
        }

        private static string[] TrimBindingsBehindCommandBinding(string[] bindings)
        {
            int index = -1;
            for (int i = 0; i < bindings.Length; ++i)
            {
                if (bindings[i].IsCommandBinding())
                {
                    index = i;
                    break;
                }
            }
            return index == -1 ? bindings : bindings.Take(index + 1).ToArray();
        }
    }
}
