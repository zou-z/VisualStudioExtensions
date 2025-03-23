using FindInViewModel.Implement;

namespace FindInViewModel.Service
{
    public static class AnalyzeServiceFactory
    {
        public static IAnalyzeService Create()
        {
            return new AnalyzeService();
        }
    }
}
