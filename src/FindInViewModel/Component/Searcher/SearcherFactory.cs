using FindInViewModel.Component.Searcher.Searchers;
using FindInViewModel.Extension;

namespace FindInViewModel.Component.Searcher
{
    internal static class SearcherFactory
    {
        public static ISearcher Create(string bindingText)
        {
            if (bindingText.IsCommandBinding())
            {
                return (commandSearcher ??= new CommandSearcher());
            }
            return variableSearcher ??= new VariableSearcher();
        }

        private static ISearcher? commandSearcher = null;
        private static ISearcher? variableSearcher = null;
    }
}
