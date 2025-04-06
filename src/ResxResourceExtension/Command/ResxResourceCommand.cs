using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.ProjectSystem.Query;
using Microsoft.VisualStudio.Shell;
using ResxResourceExtension.Model;
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
            try
            {
                await ExecuteAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                await Extensibility.Shell().ShowPromptAsync(ex.Message, PromptOptions.OK, cancellationToken);
            }
        }

        private async Task ExecuteAsync(IClientContext context, CancellationToken cancellationToken)
        {
            var activeProject = await context.GetActiveProjectAsync(t => t.With(t => t.Name), cancellationToken);
            var activeProjectName = activeProject?.Name;
            if (string.IsNullOrEmpty(activeProjectName))
            {
                throw new Exception("Get active project failed");
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            ResxResourceWindow.ShowWindow(
                activeProjectName!,
                GetProjectResourcesAsync,
                cancellationToken);
        }

        private async Task<ProjectModel[]> GetProjectResourcesAsync(CancellationToken cancellationToken)
        {
            var result = await Extensibility.Workspaces().QueryProjectsAsync(
                t => t.With(t => t.Name)
                    .With(t => t.Files.Where(t => t.FileName.EndsWith(".resx") || t.FileName.EndsWith(".Designer.cs")))
                , cancellationToken);

            if (result != null)
            {
                var findResult = result.Where(t => t.Files.Count > 0).ToList();
                var projects = findResult.Select(t => new ProjectModel(t.Name, [.. t.Files.Select(t => t.Path)])).ToList();
                projects.Sort();
                return [.. projects];
            }
            return [];
        }

        private const string guidSHLMainMenu = "d309f791-903f-11d0-9efc-00a0c911004f";
        private const uint IDG_VS_PROJ_SETTINGS = 0x014D;
        private const ushort defaultPriority = 0x0600;
    }
}
