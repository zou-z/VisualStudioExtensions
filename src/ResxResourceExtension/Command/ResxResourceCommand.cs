using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.ProjectSystem.Query;
using ResxResourceExtension.View;

namespace ResxResourceExtension.Command
{
    [VisualStudioContribution]
    internal class ResxResourceCommand : Microsoft.VisualStudio.Extensibility.Commands.Command
    {
        public ResxResourceCommand()
        {
        }

        public override CommandConfiguration CommandConfiguration => new(displayName: "%ResxResourceCommand.DisplayName%")
        {
            Placements =
            [
                CommandPlacement.VsctParent(new Guid(guidSHLMainMenu), IDG_VS_PROJ_SETTINGS, defaultPriority),
            ],
            Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.None),
        };

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return base.InitializeAsync(cancellationToken);
        }

        public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {
            var toolWindow = Extensibility.Shell().GetToolWindow<ResxResourceWindow>();
            if (toolWindow is ResxResourceWindow resxResourceWindow)
            {
                var projectName = await GetActiveProjectNameAsync(context, cancellationToken);
                resxResourceWindow.UpdateActiveProject(projectName);
                await resxResourceWindow.ShowAsync(true, cancellationToken);
            }
        }

        private async Task<string> GetActiveProjectNameAsync(IClientContext context, CancellationToken cancellationToken)
        {
            var activeProject = await context.GetActiveProjectAsync(t => t.With(t => t.Name), cancellationToken);
            return activeProject?.Name ?? string.Empty;
        }

        private const string guidSHLMainMenu = "d309f791-903f-11d0-9efc-00a0c911004f";
        private const uint IDG_VS_PROJ_SETTINGS = 0x014D;
        private const ushort defaultPriority = 0x0600;
    }
}
