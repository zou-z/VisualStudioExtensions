using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ResxResource.Resource;
using ResxResourceExtension.Interface;
using ResxResourceExtension.Model;

namespace ResxResourceExtension.ViewModel
{
    internal class ResxResourceListViewModel : ObservableObject, IResourceList
    {
        public ResxResourceListViewModel()
        {
            UpdateResourceCommand = new RelayCommand<ResourceModel>(UpdateResource);
            FilterResourcesCommand = new RelayCommand(FilterResourcesAsynchronous);
            UpdateSelectedCountCommand = new RelayCommand(UpdateSelectedCount);
            SelectAllCommand = new RelayCommand(SelectAll);
            ClearSelectedCommand = new RelayCommand(ClearSelected);
            DeleteSelectedCommand = new RelayCommand(DeleteSelected);
            CopyToClipboardCommand = new RelayCommand(CopyToClipboard);
        }

        public bool IsLoadingResources
        {
            get => isLoadingResources;
            set
            {
                SetProperty(ref isLoadingResources, value);
                _ = resourceImport?.UpdateImportExecutableAsync();
            }
        }

        public List<ResourceModel> Resources
        {
            get => resources;
            set => SetProperty(ref resources, value);
        }

        public int SelectedCount
        {
            get => selectedCount;
            set => SetProperty(ref selectedCount, value);
        }

        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        public RelayCommand<ResourceModel> UpdateResourceCommand { get; }

        public RelayCommand FilterResourcesCommand { get; }

        public RelayCommand UpdateSelectedCountCommand { get; }

        public RelayCommand SelectAllCommand { get; }

        public RelayCommand ClearSelectedCommand { get; }

        public RelayCommand DeleteSelectedCommand { get; }

        public RelayCommand CopyToClipboardCommand { get; }

        public void Initialize(IResourceImport? resourceImport)
        {
            this.resourceImport = resourceImport;
        }

        public ResourceModel[] GetExistedResources()
        {
            return allResources;
        }

        public string[] GetResourceFiles()
        {
            return resourceFiles;
        }

        public string GetModifier()
        {
            return modifier;
        }

        public void AppendResources(ResourceModel[] resources)
        {
            var list = new List<ResourceModel>(allResources.Length + resources.Length);
            list.AddRange(allResources);
            list.AddRange(resources);
            allResources = [.. list];
            FilterResourcesAsynchronous();
        }

        public void LoadResources(string[] resourceFiles)
        {
            IsLoadingResources = true;
            _ = Task.Run(() =>
            {
                this.resourceFiles = resourceFiles;
                allResources = [.. ResourceReader.Read(resourceFiles, out modifier)
                .Select(t => new ResourceModel(t.Key, t.NeutralText, t.EnglishText))];
                FilterResources();

                IsLoadingResources = false;
            });
        }

        private void FilterResources()
        {
            Resources = [.. allResources.Where(
                t => t.Key.Contains(SearchText) ||
                     t.NeutralText.Contains(SearchText)||
                     t.EnglishText.Contains(SearchText))];
            UpdateSelectedCount();
        }

        private void FilterResourcesAsynchronous()
        {
            IsLoadingResources = true;
            _ = Task.Run(() =>
            {
                FilterResources();
                IsLoadingResources = false;
            });
        }

        private void UpdateResource(ResourceModel? resource)
        {
            if (resource?.IsKeyModified == true)
            {
                ResourceUpdater.UpdateResourceDesignerKey(resourceFiles, resource.LastKey, resource.Key);
            }
            else if (resource?.IsNeutralTextModified == true)
            {
                ResourceUpdater.UpdateNeutralResourceValue(resourceFiles, resource.Key, resource.LastNeutralText, resource.NeutralText);
            }
            else if (resource?.IsEnglishTextModified == true)
            {
                ResourceUpdater.UpdateEnglishResourceValue(resourceFiles, resource.Key, resource.LastEnglishText, resource.EnglishText);
            }
            resource?.ResetModify();
        }

        private void UpdateSelectedCount()
        {
            SelectedCount = Resources.Where(t => t.IsSelected).Count();
        }

        private void SelectAll()
        {
            Resources.ForEach(t => t.IsSelected = true);
            UpdateSelectedCount();
        }

        private void ClearSelected()
        {
            Resources.ForEach(t => t.IsSelected = false);
            UpdateSelectedCount();
        }

        private void DeleteSelected()
        {
            // TODO
        }

        private void CopyToClipboard()
        {
            // TODO
        }

        private IResourceImport? resourceImport = null;
        private List<ResourceModel> resources = [];
        private int selectedCount = 0;
        private string searchText = string.Empty;
        private bool isLoadingResources = false;
        private ResourceModel[] allResources = [];
        private string[] resourceFiles = [];
        private string modifier = string.Empty;
    }
}
