namespace ResxResourceExtension.View;

using Microsoft.VisualStudio.Extensibility.UI;
using ResxResourceExtension.Model;
using ResxResourceExtension.ViewModel;
using System.Threading;
using System.Threading.Tasks;

internal class ResxResourceWindowContent(
    Func<CancellationToken, Task<ProjectModel[]>> getProjectsCallback,
    Action<string> showPromptCallback)
    : RemoteUserControl(new ResxResourceWindowContentViewModel(getProjectsCallback, showPromptCallback))
{
    public void UpdateActiveProject(string projectName)
    {
        if (DataContext is ResxResourceWindowContentViewModel viewModel)
        {
            viewModel.UpdateActiveProject(projectName);
        }
    }

    public override async Task ControlLoadedAsync(CancellationToken cancellationToken)
    {
        if (DataContext is ResxResourceWindowContentViewModel viewModel)
        {
            await viewModel.OnFirstLoadedAsync(cancellationToken);
        }
    }
}
