using FindInViewModel.Component.Searcher;
using FindInViewModel.Extension;
using FindInViewModel.Model;
using FindInViewModel.Model.Search;
using FindInViewModel.Service;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FindInViewModel.Implement
{
    internal class SearchService : ISearchService
    {
        public async Task<FilePosition?> FindAsync(
            string fromProjectName,
            string fromFileName,
            string[] bindings,
            FindFilesAsyncFunc findFilesAsyncFunc,
            CancellationToken cancellationToken)
        {
            bindings = TrimBindingsBehindCommandBinding(bindings);

            SearchResult? result = new(fromProjectName, fromFileName.GetViewModelName(), FilePosition.Empty);
            for (int i = 0; i < bindings.Length; ++i)
            {
                var targetFileName = $"{result!.VariableTypeName}.cs";
                fromProjectName = result.FromProjectName;
                do
                {
                    result = await SearcherFactory.Create(bindings[i])
                        .SearchAsync(new SearchContext(
                            fromProjectName,
                            targetFileName,
                            bindings[i],
                            findFilesAsyncFunc,
                            cancellationToken));
                }
                while (result == null && ++i < bindings.Length);
            }

            return result?.FilePosition;
        }

        public async Task<FilePosition?> FindAsync(
            string fromProjectName,
            string fromFileName,
            FindFilesAsyncFunc findFilesAsyncFunc,
            CancellationToken cancellationToken)
        {
            var targetFileName = $"{fromFileName.GetViewModelName()}.cs";
            var result = await SearcherFactory.Create(string.Empty)
                .SearchAsync(new SearchContext(
                    fromProjectName,
                    targetFileName,
                    string.Empty,
                    findFilesAsyncFunc,
                    cancellationToken));
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
