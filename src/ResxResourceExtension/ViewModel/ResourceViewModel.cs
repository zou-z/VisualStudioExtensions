using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.Extensibility.UI;
using ResxResource.Resource;
using ResxResourceExtension.Model;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace ResxResourceExtension.ViewModel
{
    [DataContract]
    internal class ResourceViewModel : NotifyPropertyChangedObject
    {
        public ResourceViewModel()
        {
            ReloadResourcesCommand = new AsyncCommand(ReloadResourcesAsync);
            FilterResourcesCommand = new AsyncCommand(FilterResourcesAsync);
            SelectAllCommand = new AsyncCommand(SelectAllAsync);
            ClearSelectedCommand = new AsyncCommand(ClearSelectedAsync);
            SaveChangesCommand = new AsyncCommand(SaveChangesAsync);
            DeleteSelectedCommand = new AsyncCommand(DeleteSelectedAsync);
            CopySelectedCommand = new AsyncCommand(CopySelectedAsync);
            UpdateSelectedResourceCountCommand = new AsyncCommand(UpdateSelectedResourceCountAsync);
            UpdateCommandStatus();
        }

        [DataMember]
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        [DataMember]
        public ResourceModel[]? Resources
        {
            get => resources;
            set
            {
                if (SetProperty(ref resources, value))
                {
                    ResourceCount = resources == null ? 0 : resources.Length;
                    SelectedResourceCount = 0;
                    UpdateCommandStatus();
                }
            }
        }

        [DataMember]
        public int ResourceCount
        {
            get => resourceCount;
            set => SetProperty(ref resourceCount, value);
        }

        [DataMember]
        public int SelectedResourceCount
        {
            get => selectedResourceCount;
            set
            {
                if (SetProperty(ref selectedResourceCount, value))
                {
                    UpdateCommandStatus();
                }
            }
        }

        [DataMember]
        public AsyncCommand ReloadResourcesCommand { get; }

        [DataMember]
        public AsyncCommand FilterResourcesCommand { get; }

        [DataMember]
        public AsyncCommand SelectAllCommand { get; }

        [DataMember]
        public AsyncCommand ClearSelectedCommand { get; }

        [DataMember]
        public AsyncCommand SaveChangesCommand { get; }

        [DataMember]
        public AsyncCommand DeleteSelectedCommand { get; }

        [DataMember]
        public AsyncCommand CopySelectedCommand { get; }

        [DataMember]
        public AsyncCommand UpdateSelectedResourceCountCommand { get; }

        public void Cleanup()
        {
            SearchText = string.Empty;
            Resources = null;

            resourceFiles = null;
            allResources = null;
            modifier = string.Empty;
        }

        public void LoadResources(string[] resourceFiles)
        {
            if (!resourceFiles.Equals(this.resourceFiles))
            {
                this.resourceFiles = resourceFiles;
                _ = LoadingTipViewModel.Instance.ExecuteWithTipAsync(LoadResources);
            }
        }

        public string GetModifier() => modifier;

        private async Task ReloadResourcesAsync(object? parameter, CancellationToken token)
        {
            await LoadingTipViewModel.Instance.ExecuteWithTipAsync(LoadResources);
        }

        private async Task FilterResourcesAsync(object? parameter, CancellationToken token)
        {
            await LoadingTipViewModel.Instance.ExecuteWithTipAsync(FilterResources);
        }

        private Task SelectAllAsync(object? parameter, CancellationToken token)
        {
            if (Resources != null)
            {
                Array.ForEach(Resources, t => t.IsSelected = true);
                SelectedResourceCount = Resources.Length;
            }
            return Task.CompletedTask;
        }

        private Task ClearSelectedAsync(object? parameter, CancellationToken token)
        {
            if (Resources != null)
            {
                Array.ForEach(Resources, t => t.IsSelected = false);
                SelectedResourceCount = 0;
            }
            return Task.CompletedTask;
        }

        private async Task SaveChangesAsync(object? parameter, CancellationToken token)
        {
            await LoadingTipViewModel.Instance.ExecuteWithTipAsync(SaveChanges);
        }

        private async Task DeleteSelectedAsync(object? parameter, IClientContext context, CancellationToken token)
        {
            if (!await LoadingTipViewModel.Instance.ExecuteWithTipAsync(DeleteSelected))
            {
                await context.Extensibility.Shell().ShowPromptAsync("Delete failed, please refresh the resources", PromptOptions.OK, token);
            }
        }

        private async Task CopySelectedAsync(object? parameter, IClientContext context, CancellationToken token)
        {
            await CopySelectedAsync(context, token);
        }

        private Task UpdateSelectedResourceCountAsync(object? parameter, CancellationToken token)
        {
            UpdateSelectedResourceCount();
            return Task.CompletedTask;
        }

        private void LoadResources()
        {
            if (resourceFiles == null || resourceFiles.Length == 0)
            {
                allResources = [];
            }
            else
            {
                var resourceItems = ResourceReader.Read(resourceFiles, out modifier);
                allResources = [.. resourceItems.Select(t => new ResourceModel(t.Key, t.NeutralText, t.EnglishText))];
            }

            FilterResources();
        }

        private void FilterResources()
        {
            if (allResources == null)
                return;

            if (string.IsNullOrEmpty(SearchText))
            {
                Resources = [.. allResources];
            }
            else
            {
                Resources = [..allResources.Where(
                    t => t.Key.Contains(SearchText) ||
                         t.NeutralText.Contains(SearchText) ||
                         t.EnglishText.Contains(SearchText)
                )];
            }

            UpdateSelectedResourceCount();
        }

        private void SaveChanges()
        {
            if (Resources == null || resourceFiles == null || resourceFiles.Length == 0)
                return;

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

        private bool DeleteSelected()
        {
            if (Resources == null || resourceFiles == null || resourceFiles.Length == 0)
                return true;

            var items = Resources.Where(t => t.IsSelected);
            var keys = items.Select(t => t.Key).ToArray();
            if (keys.Length == 0)
            {
                return true;
            }

            if (ResourceDeleter.Delete(resourceFiles, keys))
            {
                allResources = [.. allResources.Except(items)];
                Resources = [.. Resources.Except(items)];
                return true;
            }
            return false;
        }

        private async Task CopySelectedAsync(IClientContext context, CancellationToken token)
        {
            if (Resources == null || resourceFiles == null || resourceFiles.Length == 0)
                return;

            var selectedResources = Resources.Where(t => t.IsSelected);
            if (selectedResources.Count() == 0)
            {
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
                await context.Extensibility.Shell().ShowPromptAsync($"Copy failed, {ex.Message}", PromptOptions.OK, token);
            }
        }

        private void UpdateSelectedResourceCount()
        {
            SelectedResourceCount = Resources == null
                ? 0
                : Resources.Count(t => t.IsSelected);
        }

        private void UpdateCommandStatus()
        {
            var hasSelected = SelectedResourceCount > 0;
            SelectAllCommand.CanExecute = ResourceCount > 0 && SelectedResourceCount < ResourceCount;
            ClearSelectedCommand.CanExecute = hasSelected;
            DeleteSelectedCommand.CanExecute = hasSelected;
            CopySelectedCommand.CanExecute = hasSelected;
        }

        private string[]? resourceFiles = null;
        private ResourceModel[]? allResources = null;
        private string modifier = string.Empty;

        private string searchText = string.Empty;
        private ResourceModel[]? resources = null;
        private int resourceCount = 0;
        private int selectedResourceCount = 0;
    }
}
