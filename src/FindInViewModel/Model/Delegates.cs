using FindInViewModel.Model.Search;
using System.Threading;
using System.Threading.Tasks;

namespace FindInViewModel.Model
{
    public delegate Task<FileSource[]> FindFilesAsyncFunc(
        string fromProjectName,
        string fileName,
        CancellationToken cancellationToken);
}
