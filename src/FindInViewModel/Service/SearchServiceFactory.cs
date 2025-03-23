using FindInViewModel.Implement;

namespace FindInViewModel.Service
{
    public static class SearchServiceFactory
    {
        public static ISearchService Create()
        {
            return new SearchService();
        }
    }
}
