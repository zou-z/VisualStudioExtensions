using FindInViewModel.Service;
using System.Linq;

namespace FindInViewModel.Implement
{
    internal class AnalyzeService : IAnalyzeService
    {
        public string[] GetBindings(string bindingText)
        {
            return [.. bindingText.Split('.').Where(t => !string.IsNullOrEmpty(t))];
        }
    }
}
