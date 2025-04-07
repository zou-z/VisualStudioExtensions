using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ResxResourceExtension.Model;

namespace ResxResourceExtension.ViewModel
{
    internal class ResxResourceViewModel : ObservableObject
    {
        public ResxResourceViewModel(Func<Task<SolutionModel>> getSolutionDataAsyncFunc)
        {
            this.getSolutionDataAsyncFunc = getSolutionDataAsyncFunc;

            ListViewModel = new ResxResourceListViewModel();
            ImportViewModel = new ResxResourceImportViewModel(() => IsShowImportView = false);
            ListViewModel.Initialize(ImportViewModel);
            ImportViewModel.Initialize(ListViewModel);

            LoadedCommand = new RelayCommand(Loaded);
            SwitchImportViewCommand = new RelayCommand(() => IsShowImportView = !IsShowImportView);
        }

        public ProjectModel[] Projects
        {
            get => projects;
            set => SetProperty(ref projects, value);
        }

        public ProjectModel? SelectedProject
        {
            get => selectedProject;
            set
            {
                SetProperty(ref selectedProject, value);
                OnSelectedProjectNameChanged();
            }
        }

        public bool IsShowImportView
        {
            get => isShowImportView;
            set => SetProperty(ref isShowImportView, value);
        }

        public ResxResourceListViewModel ListViewModel { get; }

        public ResxResourceImportViewModel ImportViewModel { get; }

        public RelayCommand LoadedCommand { get; }

        public RelayCommand SwitchImportViewCommand { get; }

        private void Loaded()
        {
            _ = Task.Run(async () =>
            {
                var solution = await getSolutionDataAsyncFunc();
                Projects = solution.Projects;
                SelectedProject = Projects.FirstOrDefault(t => t.Name == solution.ActiveProjectName);
            });
        }

        private void OnSelectedProjectNameChanged()
        {
            ListViewModel.LoadResources(SelectedProject?.ResourceFiles ?? []);
        }

        private readonly Func<Task<SolutionModel>> getSolutionDataAsyncFunc;
        private ProjectModel[] projects = [];
        private ProjectModel? selectedProject = null;
        private bool isShowImportView = false;
    }
}
