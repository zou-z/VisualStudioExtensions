using FindInViewModel.Component.Searcher.Searchers;
using FindInViewModel.Extension;

namespace FindInViewModel.Component.Searcher
{
    internal static class SearcherFactory
    {
        public static ISearcher Create(string bindingText)
        {
            if (string.IsNullOrEmpty(bindingText))
            {
                return viewModelSearcher ??= new ViewModelSearcher();
            }
            if (bindingText.IsCommandBinding())
            {
                return commandSearcher ??= new CommandSearcher();
            }
            return propertySearcher ??= new PropertySearcher();
        }

        private static ISearcher? commandSearcher = null;
        private static ISearcher? propertySearcher = null;
        private static ISearcher? viewModelSearcher = null;
    }
}
