using Microsoft.VisualStudio.Extensibility.UI;
using Microsoft.VisualStudio.ProjectSystem.Query;
using Microsoft.VisualStudio.Shell;
using ResxResource.Resource;
using ResxResourceExtension.Model;
using System.Runtime.Serialization;
using System.Windows;

namespace ResxResourceExtension.ViewModel
{
    [DataContract]
    internal class ResxResourceWindowContentViewModel : ResxResourceWindowContentViewModelBase
    {
        public ResxResourceWindowContentViewModel(
            Func<CancellationToken, Task<ProjectModel[]>> getProjectsCallback,
            Action<string> showPromptCallback)
            : base(getProjectsCallback, showPromptCallback)
        {
            ImportCommand = new AsyncCommand(ImportAsync);
            RefreshCommand = new AsyncCommand(RefreshAsync);
            SearchCommand = new AsyncCommand(SearchAsync);
            SaveChangesCommand = new AsyncCommand(SaveChangesAsync);
            DeleteSelectedCommand = new AsyncCommand(DeleteSelectedAsync);
            CopySelectedCommand = new AsyncCommand(CopySelectedAsync);
            SelectAllCommand = new AsyncCommand(SelectAllAsync);
            ClearSelectedCommand = new AsyncCommand(ClearSelectedAsync);
        }

        [DataMember]
        public AsyncCommand ImportCommand { get; }

        [DataMember]
        public AsyncCommand RefreshCommand { get; }

        [DataMember]
        public AsyncCommand SearchCommand { get; }

        [DataMember]
        public AsyncCommand SaveChangesCommand { get; }

        [DataMember]
        public AsyncCommand DeleteSelectedCommand { get; }

        [DataMember]
        public AsyncCommand CopySelectedCommand { get; }

        [DataMember]
        public AsyncCommand SelectAllCommand { get; }

        [DataMember]
        public AsyncCommand ClearSelectedCommand { get; }

        private async Task ImportAsync(object? arg1, CancellationToken token)
        {
            await ExecuteAsync(Import);
        }

        private async Task RefreshAsync(object? parameter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(SelectedProjectName))
            {
                showPromptCallback("Please select a project.");
                return;
            }

            await LoadResourcesAsync();
        }

        private async Task SearchAsync(object? parameter, CancellationToken token)
        {
            await FilterResourcesAsync();
        }

        private async Task SaveChangesAsync(object? arg1, CancellationToken token)
        {
            await ExecuteAsync(SaveChanges);
        }

        private async Task DeleteSelectedAsync(object? arg1, CancellationToken token)
        {
            if (await ExecuteAsync(DeleteSelected))
            {
                await LoadResourcesAsync();
            }
        }

        private async Task CopySelectedAsync(object? arg1, CancellationToken token)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            CopySelected();
        }

        private Task SelectAllAsync(object? parameter, CancellationToken token)
        {
            if (Resources != null)
            {
                Array.ForEach(Resources, t => t.IsSelected = true);
            }
            return Task.CompletedTask;
        }

        private Task ClearSelectedAsync(object? parameter, CancellationToken token)
        {
            if (Resources != null)
            {
                Array.ForEach(Resources, t => t.IsSelected = false);
            }
            return Task.CompletedTask;
        }

        private void Import()
        {
            if (string.IsNullOrEmpty(ImportText))
            {
                showPromptCallback("Please enter the resources text.");
                return;
            }

            if (string.IsNullOrEmpty(SelectedProjectName))
            {
                showPromptCallback("Please select project first.");
                return;
            }

            if (!ResourceImporter.GetImportResourcesFromText(ImportText, out var resources))
            {
                showPromptCallback("The resources text is uncompleted.");
                return;
            }

            var resourceFiles = GetCurrentResourceFiles();
            if (!ResourceImporter.Import(resourceFiles, resources, modifier))
            {
                showPromptCallback("Import resources failed.");
                return;
            }

            ImportText = string.Empty;
        }

        private void SaveChanges()
        {
            if (Resources != null)
            {
                var resourceFiles = GetCurrentResourceFiles();
                if (resourceFiles.Length > 0)
                {
                    foreach (var resource in Resources)
                    {
                        var isModified = false;
                        if (resource.IsKeyModified)
                        {
                            ResourceUpdater.UpdateKey(resourceFiles, resource.LastKey, resource.Key);
                            isModified = true;
                        }
                        if (resource.IsNeutralTextModified)
                        {
                            ResourceUpdater.UpdateNeutralText(resourceFiles, resource.Key, resource.LastNeutralText, resource.NeutralText);
                            isModified = true;
                        }
                        if (resource.IsEnglishTextModified)
                        {
                            ResourceUpdater.UpdateEnglishText(resourceFiles, resource.Key, resource.LastEnglishText, resource.EnglishText);
                            isModified = true;
                        }

                        if (isModified)
                        {
                            resource.ResetModifyStatus();
                        }
                    }
                }
            }
        }

        private bool DeleteSelected()
        {
            if (Resources != null)
            {
                var resourceFiles = GetCurrentResourceFiles();
                if (resourceFiles.Length > 0)
                {
                    var keys = Resources.Where(t => t.IsSelected).Select(t => t.Key).ToArray();
                    if (keys.Length == 0)
                    {
                        showPromptCallback("Please select resource item first.");
                        return false;
                    }

                    ResourceDeleter.Delete(resourceFiles, keys);
                }
            }
            return true;
        }

        private void CopySelected()
        {
            if (Resources != null)
            {
                var resourceFiles = GetCurrentResourceFiles();
                if (resourceFiles.Length > 0)
                {
                    var selectedResources = Resources.Where(t => t.IsSelected);
                    if (selectedResources.Count() == 0)
                    {
                        showPromptCallback("Please select resource item first.");
                        return;
                    }

                    var resourceTexts = selectedResources.Select(t => string.Join(Environment.NewLine, t.Key, t.NeutralText, t.EnglishText));
                    var text = string.Join($"{Environment.NewLine}{Environment.NewLine}", resourceTexts);
                    try
                    {
                        Clipboard.SetText(text);
                    }
                    catch (Exception ex)
                    {
                        showPromptCallback($"Copy failed, {ex.Message}");
                    }
                }
            }
        }
    }
}
