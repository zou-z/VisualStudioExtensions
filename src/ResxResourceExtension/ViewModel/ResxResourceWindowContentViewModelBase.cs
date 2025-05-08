using Microsoft.VisualStudio.Extensibility.UI;
using ResxResource.Resource;
using ResxResourceExtension.Model;
using System.Runtime.Serialization;

namespace ResxResourceExtension.ViewModel
{
    [DataContract]
    internal abstract class ResxResourceWindowContentViewModelBase(
        Func<CancellationToken, Task<ProjectModel[]>> getProjectsCallback,
        Action<string> showPromptCallback) : NotifyPropertyChangedObject
    {
        [DataMember]
        public string[]? ProjectNames
        {
            get => projectNames;
            set => SetProperty(ref projectNames, value);
        }

        [DataMember]
        public string? SelectedProjectName
        {
            get => selectedProjectName;
            set
            {
                if (SetProperty(ref selectedProjectName, value))
                {
                    _ = LoadResourcesAsync();
                }
            }
        }

        [DataMember]
        public int SelectedTabIndex
        {
            get => selectedTabIndex;
            set
            {
                SetProperty(ref selectedTabIndex, value);
                if (value == 1 && Resources == null && !string.IsNullOrEmpty(SelectedProjectName))
                {
                    _ = LoadResourcesAsync();
                }
            }
        }

        [DataMember]
        public string ImportText
        {
            get => importText;
            set => SetProperty(ref importText, value);
        }

        [DataMember]
        public ResourceModel[]? Resources
        {
            get => resources;
            set
            {
                SetProperty(ref resources, value);
                ResourceCount = value == null ? 0 : value.Length;
            }
        }

        [DataMember]
        public int ResourceCount
        {
            get => resourceCount;
            set => SetProperty(ref resourceCount, value);
        }

        [DataMember]
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        [DataMember]
        public LoadingTipViewModel LoadingTip { get; } = new LoadingTipViewModel();

        public async Task OnFirstLoadedAsync(CancellationToken cancellationToken)
        {
            LoadingTip.Show();

            projects = await getProjectsCallback(cancellationToken);
            ProjectNames = [.. projects.Select(t => t.Name)];
            UpdateSelectedProject();

            LoadingTip.Hide();
        }

        public void UpdateActiveProject(string projectName)
        {
            activeProjectName = projectName;
            UpdateSelectedProject();
        }

        protected async Task ExecuteAsync(Action action)
        {
            LoadingTip.Show();
            await Task.Run(action);
            LoadingTip.Hide();
        }

        protected async Task<T> ExecuteAsync<T>(Func<T> action)
        {
            LoadingTip.Show();
            var result = await Task.Run(action);
            LoadingTip.Hide();
            return result;
        }

        protected async Task LoadResourcesAsync()
        {
            await ExecuteAsync(LoadResources);
        }

        protected async Task FilterResourcesAsync()
        {
            await ExecuteAsync(FilterResources);
        }

        protected string[] GetCurrentResourceFiles() => currentProject?.ResourceFiles ?? [];

        private void LoadResources()
        {
            if (string.IsNullOrEmpty(SelectedProjectName))
            {
                projectResources = [];
            }
            else
            {
                currentProject = projects?.FirstOrDefault(t => t.Name == SelectedProjectName);
                if (currentProject == null)
                {
                    showPromptCallback($"Project '{SelectedProjectName}' not found.");
                    return;
                }

                if (currentProject.ResourceFiles.Length == 0)
                {
                    projectResources = [];
                }
                else
                {
                    var resourceItems = ResourceReader.Read(currentProject.ResourceFiles, out modifier);
                    projectResources = [.. resourceItems.Select(t => new ResourceModel(t.Key, t.NeutralText, t.EnglishText))];
                }
            }

            FilterResources();
        }

        private void FilterResources()
        {
            if (projectResources == null)
                return;

            if (string.IsNullOrEmpty(SearchText))
            {
                Resources = [.. projectResources];
            }
            else
            {
                Resources = [..projectResources.Where(
                    t => t.Key.Contains(SearchText) ||
                         t.NeutralText.Contains(SearchText) ||
                         t.EnglishText.Contains(SearchText)
                )];
            }
        }

        private void UpdateSelectedProject()
        {
            SelectedProjectName = ProjectNames == null || string.IsNullOrEmpty(activeProjectName)
                ? null
                : ProjectNames.FirstOrDefault(t => t == activeProjectName);
        }

        protected readonly Action<string> showPromptCallback = showPromptCallback;
        protected string modifier = "public";

        private readonly Func<CancellationToken, Task<ProjectModel[]>> getProjectsCallback = getProjectsCallback;
        private string? activeProjectName = null;
        private ProjectModel[]? projects = null;
        private ProjectModel? currentProject = null;
        private ResourceModel[]? projectResources = null;

        private string[]? projectNames = null;
        private string? selectedProjectName = null;
        private int selectedTabIndex = -1;
        private string importText = string.Empty;
        private ResourceModel[]? resources = null;
        private int resourceCount = 0;
        private string searchText = string.Empty;
    }
}
