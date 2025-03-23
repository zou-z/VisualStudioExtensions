using EnvDTE;
using EnvDTE80;
using FindInViewModel.Model.Search;
using FindInViewModel.Service;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.Shell;
using System.IO;

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
            dte = await asyncServiceProviderInjection.GetServiceAsync();
            var document = dte.ActiveDocument;

            // 获取当前选中文本
            var selection = document?.Selection as TextSelection;
            var selectedText = selection?.Text?.Trim();
            if (string.IsNullOrEmpty(selectedText))
            {
                throw new Exception("Please select the text first");
            }

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

            // 查找文件位置
            selectedText = selectedText!.Split(Environment.NewLine.ToCharArray()).First();
            var bindings = AnalyzeServiceFactory.Create().GetBindings(selectedText!);
            var searchService = SearchServiceFactory.Create();
            var filePosition = searchService.FindAsync(currentProjectName!, currentFileName!, bindings, FindFiles);

            // 打开并选中
            if (filePosition != null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
                OpenFile(filePosition);
            }
            else
            {
                throw new Exception("Not found");
            }

            dte = null;
        }

        private FileSource[] FindFiles(string fromProjectName, string fileName)
        {
            if (dte == null)
            {
                return [];
            }

            var list = new List<FileSource>();
            do
            {
                var projects = dte.Solution?.Projects;
                if (projects == null)
                {
                    break;
                }

                foreach (Project project in projects)
                {
                    var projectName = project?.Name;
                    var projectFullName = project?.FullName;
                    if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(projectFullName))
                    {
                        continue;
                    }

                    var projectPath = Path.GetDirectoryName(projectFullName);
                    var filePaths = new List<string>();
                    FindFilePaths(filePaths, projectPath, fileName, ["bin", "obj"]);
                    if (filePaths.Count == 0)
                    {
                        continue;
                    }

                    var fileSource = new FileSource(projectName!, [.. filePaths]);
                    if (projectName == fromProjectName)
                    {
                        list.Insert(0, fileSource);
                    }
                    else
                    {
                        list.Add(fileSource);
                    }
                }
            } while (false);
            return [.. list];
        }

        private void FindFilePaths(List<string> filePaths, string folderPath, string fileName, string[] ignoredFolders)
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            foreach (var directory in directoryInfo.GetDirectories())
            {
                if (!ignoredFolders.Contains(directory.Name))
                {
                    FindFilePaths(filePaths, directory.FullName, fileName, ignoredFolders);
                }
            }
            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Name == fileName)
                {
                    filePaths.Add(file.FullName);
                }
            }
        }

        private void OpenFile(FilePosition filePosition)
        {
            if (dte == null)
            {
                return;
            }

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
        private DTE2? dte = null;
    }
}
