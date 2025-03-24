using FindInViewModel.Model.Search;
using System.Threading.Tasks;

namespace FindInViewModel.Component.Searcher
{
    internal interface ISearcher
    {
        Task<SearchResult?> SearchAsync(SearchContext context);
    }
}
