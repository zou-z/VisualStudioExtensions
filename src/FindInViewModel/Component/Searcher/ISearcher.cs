using FindInViewModel.Model.Search;

namespace FindInViewModel.Component.Searcher
{
    internal interface ISearcher
    {
        SearchResult? Search(SearchContext context);
    }
}
