using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

namespace ResxResourceExtension.View;

[VisualStudioContribution]
public class ResxResourceWindow : ToolWindow
{
    public ResxResourceWindow()
    {
        Title = "Resx Resource";
        content = new ResxResourceWindowContent();
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

    private readonly ResxResourceWindowContent content;
}
