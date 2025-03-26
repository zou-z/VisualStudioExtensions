using EnvDTE;
using EnvDTE80;
using FindInViewModel.Model.Search;
using FindInViewModel.Service;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.ProjectSystem.Query;
using Microsoft.VisualStudio.Shell;

namespace FindInViewModelExtension.Command
{
    [VisualStudioContribution]
    internal class FindInViewModelCommand(
        AsyncServiceProviderInjection<DTE, DTE2> asyncServiceProviderInjection) : Microsoft.VisualStudio.Extensibility.Commands.Command
    {
        public override CommandConfiguration CommandConfiguration => new(displayName: "%FindInViewModelCommand.DisplayName%")
        {
            Placements =
            [
                CommandPlacement.VsctParent(new Guid(guidSHLMainMenu), IDG_VS_CODEWIN_LANGUAGE, defaultPriority)
            ],
            Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.None),
            VisibleWhen = ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveSelectionFileName, @"\.xaml$"),
        };

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return base.InitializeAsync(cancellationToken);
        }

        public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {
            try
            {
                await ExecuteAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await Extensibility.Shell().ShowPromptAsync(ex.Message, PromptOptions.OK, cancellationToken);
            }
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var dte = await asyncServiceProviderInjection.GetServiceAsync();
            var document = dte.ActiveDocument;

            // 获取当前项目名称
            var currentProjectName = document?.ProjectItem?.ContainingProject?.Name;
            if (string.IsNullOrEmpty(currentProjectName))
            {
                throw new Exception("Get current project name failed");
            }

            // 获取当前文件名称
            var currentFileName = document?.Name;
            if (string.IsNullOrEmpty(currentFileName))
            {
                throw new Exception("Get current file name failed");
            }

            // 获取当前选中文本
            var selection = document?.Selection as TextSelection;
            var selectedText = selection?.Text?.Trim();

            // 查找文件位置
            FilePosition? filePosition;
            var searchService = SearchServiceFactory.Create();
            if (string.IsNullOrEmpty(selectedText))
            {
                filePosition = await searchService.FindAsync(
                    currentProjectName!,
                    currentFileName!,
                    FindFilesAsync,
                    cancellationToken);
            }
            else
            {
                selectedText = selectedText!.Split(Environment.NewLine.ToCharArray()).First();
                var bindings = AnalyzeServiceFactory.Create().GetBindings(selectedText!);

                filePosition = await searchService.FindAsync(
                    currentProjectName!,
                    currentFileName!,
                    bindings,
                    FindFilesAsync,
                    cancellationToken);
            }

            // 打开并选中
            if (filePosition != null)
            {
                OpenFile(dte, filePosition);
            }
            else
            {
                throw new Exception("Not found");
            }
        }

        private async Task<FileSource[]> FindFilesAsync(
            string fromProjectName,
            string fileName,
            CancellationToken cancellationToken)
        {
            var projects = await Extensibility.Workspaces().QueryProjectsAsync(
                t => t.With(t => t.Name)
                      .With(t => t.Files.Where(t => t.FileName == fileName)),
                cancellationToken);

            var fileSources = new List<FileSource>();
            foreach (var project in projects)
            {
                if (project.Files.Count > 0)
                {
                    var fileSource = new FileSource(project.Name, [.. project.Files.Select(t => t.Path)]);
                    if (project.Name == fromProjectName)
                    {
                        fileSources.Insert(0, fileSource);
                    }
                    else
                    {
                        fileSources.Add(fileSource);
                    }
                }
            }

            return [.. fileSources];
        }

        private void OpenFile(DTE2 dte, FilePosition filePosition)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var window = dte.ItemOperations?.OpenFile(filePosition.FilePath, Constants.vsViewKindTextView);
            if (window != null)
            {
                var document = dte.ActiveDocument;
                if (document != null)
                {
                    var textDocument = (TextDocument)document.Object("TextDocument");
                    if (textDocument != null)
                    {
                        var selection = textDocument.Selection;
                        if (selection != null)
                        {
                            int line = filePosition.LineIndex + 1;
                            int startOffset = filePosition.ColumnIndex + 1;
                            int endOffset = startOffset + filePosition.ColumnLength;
                            selection.MoveToLineAndOffset(line, startOffset, false);
                            selection.MoveToLineAndOffset(line, endOffset, true);
                        }
                    }
                }
            }
        }

        private const string guidSHLMainMenu = "d309f791-903f-11d0-9efc-00a0c911004f";
        private const uint IDG_VS_CODEWIN_LANGUAGE = 0x02D0;
        private const ushort defaultPriority = 0x0600;

        private readonly AsyncServiceProviderInjection<DTE, DTE2> asyncServiceProviderInjection = asyncServiceProviderInjection;
    }
}
