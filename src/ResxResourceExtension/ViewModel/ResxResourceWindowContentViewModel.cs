using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;

namespace ResxResourceExtension.ViewModel
{
    [DataContract]
    internal class ResxResourceWindowContentViewModel : NotifyPropertyChangedObject
    {
        public ResxResourceWindowContentViewModel()
        {
            LoadCommand = new AsyncCommand(LoadAsync);
            UnloadCommand = new AsyncCommand(UnloadAsync);

            Solution = new SolutionViewModel(NotifyUpdateResources);
            Resource = new ResourceViewModel();
            Import = new ImportViewModel(
                () => Solution.SelectedProject != null,
                () => Resource.GetModifier(),
                () => Solution.SelectedProject == null ? [] : Solution.SelectedProject.ResourceFiles
            );
        }

        [DataMember]
        public int SelectedTabIndex
        {
            get => selectedTabIndex;
            set
            {
                if (SetProperty(ref selectedTabIndex, value))
                {
                    NotifyUpdateResources();
                }
            }
        }

        [DataMember]
        public SolutionViewModel Solution { get; }

        [DataMember]
        public ImportViewModel Import { get; }

        [DataMember]
        public ResourceViewModel Resource { get; }

        [DataMember]
        public LoadingTipViewModel LoadingTip => LoadingTipViewModel.Instance;

        [DataMember]
        public AsyncCommand LoadCommand { get; }

        [DataMember]
        public AsyncCommand UnloadCommand { get; }

        private async Task LoadAsync(object? parameter, IClientContext context, CancellationToken token)
        {
            if (Solution.Projects == null)
            {
                await Solution.RefreshCommand.ExecuteAsync(parameter, context, token);
            }
        }

        private Task UnloadAsync(object? parameter, CancellationToken token)
        {
            Import.Cleanup();
            Resource.Cleanup();
            Solution.Cleanup();
            return Task.CompletedTask;
        }

        private void NotifyUpdateResources()
        {
            if (SelectedTabIndex != 1)
                return;

            var resourceFiles = Solution.SelectedProject == null
                ? []
                : Solution.SelectedProject.ResourceFiles;

            Resource.LoadResources(resourceFiles);
        }

        private int selectedTabIndex = -1;
    }
}
