using ResxResourceExtension.Model;
using ResxResourceExtension.ViewModel;

namespace ResxResourceExtension.View
{
    internal class ResxResourceWindow : SingletonWindow
    {
        private ResxResourceWindow()
        {
        }

        public static void ShowWindow(Func<Task<SolutionModel>> getSolutionDataAsyncFunc)
        {
            var viewModel = new ResxResourceViewModel(getSolutionDataAsyncFunc);
            var window = Create("Resx Resource", 432, 680);
            window.Content = new ResxResourceView { DataContext = viewModel };
            window.ShowDialog();
        }
    }
}
