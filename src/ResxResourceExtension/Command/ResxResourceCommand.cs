using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
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
            await Extensibility.Shell().ShowToolWindowAsync<ResxResourceWindow>(true, cancellationToken);
        }

        private const string guidSHLMainMenu = "d309f791-903f-11d0-9efc-00a0c911004f";
        private const uint IDG_VS_PROJ_SETTINGS = 0x014D;
        private const ushort defaultPriority = 0x0600;
    }
}
