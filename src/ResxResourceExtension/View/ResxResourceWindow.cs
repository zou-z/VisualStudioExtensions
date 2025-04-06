using ResxResourceExtension.Model;
using ResxResourceExtension.ViewModel;

namespace ResxResourceExtension.View
{
    internal class ResxResourceWindow : SingletonWindow
    {
        private ResxResourceWindow()
        {
        }

        public static void ShowWindow(
            string activeProjectName,
            Func<CancellationToken, Task<ProjectModel[]>> getProjectResourcesAsyncFunc,
            CancellationToken cancellationToken)
        {
            var viewModel = new ResxResourceViewModel(
                activeProjectName,
                getProjectResourcesAsyncFunc,
                cancellationToken);
            var window = Create("Resx Resource", 432, 680);
            window.Content = new ResxResourceView { DataContext = viewModel };
            window.Show();
        }
    }
}
