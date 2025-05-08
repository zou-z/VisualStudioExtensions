namespace ResxResourceExtension.View;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.ProjectSystem.Query;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using ResxResourceExtension.Model;
using System.Threading;
using System.Threading.Tasks;

[VisualStudioContribution]
public class ResxResourceWindow : ToolWindow
{
    public ResxResourceWindow()
    {
        Title = "Resx Resource";
        content = new ResxResourceWindowContent(GetProjectsAsync, ShowPrompt);
    }

    public void UpdateActiveProject(string projectName)
    {
        content.UpdateActiveProject(projectName);
    }

    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        Placement = ToolWindowPlacement.Floating,
    };

    public override Task InitializeAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IRemoteUserControl>(content);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            content.Dispose();
        base.Dispose(disposing);
    }

    private async Task<ProjectModel[]> GetProjectsAsync(CancellationToken cancellationToken)
    {
        var result = await Extensibility.Workspaces().QueryProjectsAsync(
            t => t.With(t => t.Name)
                .With(t => t.Files.Where(
                    t => t.FileName.EndsWith(".resx") || t.FileName.EndsWith(".Designer.cs"))),
            cancellationToken
        );

        if (result == null)
            return [];

        var projects = result
            .Where(t => t.Files.Count > 0)
            .Select(t => new ProjectModel(t.Name, [.. t.Files.Select(t => t.Path)]))
            .OrderBy(t => t.Name);
        return [.. projects];
    }

    private void ShowPrompt(string message)
    {
        _ = Extensibility.Shell().ShowPromptAsync(message, PromptOptions.OK, default);
    }

    private readonly ResxResourceWindowContent content;
}
