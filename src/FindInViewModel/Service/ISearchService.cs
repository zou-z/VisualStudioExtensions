using FindInViewModel.Model;
using FindInViewModel.Model.Search;
using System.Threading;
using System.Threading.Tasks;

namespace FindInViewModel.Service
{
    public interface ISearchService
    {
        Task<FilePosition?> FindAsync(
            string fromProjectName,
            string fromFileName,
            string[] bindings,
            FindFilesAsyncFunc findFilesAsyncFunc,
            CancellationToken cancellationToken);
    }
}
