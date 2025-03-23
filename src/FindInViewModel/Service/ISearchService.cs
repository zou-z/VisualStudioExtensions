using FindInViewModel.Model;
using FindInViewModel.Model.Search;

namespace FindInViewModel.Service
{
    public interface ISearchService
    {
        FilePosition? FindAsync(string fromProjectName, string fromFileName, string[] bindings, FindFilesFunc findFilesFunc);
    }
}
