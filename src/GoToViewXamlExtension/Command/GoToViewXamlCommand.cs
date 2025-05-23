﻿using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.ProjectSystem.Query;

namespace GoToViewXamlExtension.Command
{
    [VisualStudioContribution]
    internal class GoToViewXamlCommand : Microsoft.VisualStudio.Extensibility.Commands.Command
    {
        public override CommandConfiguration CommandConfiguration => new(displayName: "%GoToViewXamlCommand.DisplayName%")
        {
            Placements =
            [
                CommandPlacement.VsctParent(new Guid(guidSHLMainMenu), IDG_VS_CODEWIN_LANGUAGE, defaultPriority)
            ],
            Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.None),
            VisibleWhen = ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveSelectionFileName, @"ViewModel\.cs$"),
        };

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return base.InitializeAsync(cancellationToken);
        }

        public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {
            try
            {
                var textView = await context.GetActiveTextViewAsync(cancellationToken) ??
                    throw new Exception("Get current text failed");
                var project = await context.GetActiveProjectAsync(t => t.With(t => t.Name), cancellationToken) ??
                    throw new Exception("Get current project failed");

                var viewModelFileTail = "ViewModel.cs";
                var fileName = Path.GetFileName(textView.FilePath);
                if (fileName?.EndsWith(viewModelFileTail) != true)
                {
                    throw new Exception("Current file is not a ViewModel file");
                }

                var name = fileName[..^viewModelFileTail.Length];
                var viewFileNames = new[] { $"{name}.xaml", $"{name}View.xaml" };

                string? viewFilePath = null;
                for (int i = 0; i < viewFileNames.Length && string.IsNullOrEmpty(viewFilePath); ++i)
                {
                    viewFilePath = await GetFilePathAsync(viewFileNames[i], project.Name, cancellationToken);
                }

                if (string.IsNullOrEmpty(viewFilePath))
                {
                    throw new Exception("Not found");
                }
                await Extensibility.Documents().OpenTextDocumentAsync(new Uri(viewFilePath), cancellationToken);
            }
            catch (Exception ex)
            {
                await Extensibility.Shell().ShowPromptAsync(ex.Message, PromptOptions.OK, cancellationToken);
            }
        }

        private async Task<string?> GetFilePathAsync(string fileName, string projectName, CancellationToken cancellationToken)
        {
            var result = await Extensibility.Workspaces().QueryProjectsAsync(
                   t => t.With(t => t.Name).With(t => t.Files.Where(t => t.FileName == fileName)),
                   cancellationToken);
            var projects = result.Where(t => t.Files.Count > 0);

            var filePath = projects.Where(t => t.Name == projectName).FirstOrDefault()?.Files.FirstOrDefault()?.Path;
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = projects.FirstOrDefault()?.Files.FirstOrDefault()?.Path;
            }
            return filePath;
        }

        private const string guidSHLMainMenu = "d309f791-903f-11d0-9efc-00a0c911004f";
        private const uint IDG_VS_CODEWIN_LANGUAGE = 0x02D0;
        private const ushort defaultPriority = 0x0600;
    }
}
