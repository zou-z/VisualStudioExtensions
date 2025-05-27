using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.UI;
using Microsoft.VisualStudio.ProjectSystem.Query;
using ResxResourceExtension.Model;
using System.Runtime.Serialization;

namespace ResxResourceExtension.ViewModel
{
    [DataContract]
    internal class SolutionViewModel : NotifyPropertyChangedObject
    {
        public SolutionViewModel(Action selectedProjectChangedCallback)
        {
            this.selectedProjectChangedCallback = selectedProjectChangedCallback;
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        [DataMember]
        public ProjectModel[]? Projects
        {
            get => projects;
            set => SetProperty(ref projects, value);
        }

        [DataMember]
        public ProjectModel? SelectedProject
        {
            get => selectedProject;
            set
            {
                if (SetProperty(ref selectedProject, value))
                {
                    selectedProjectChangedCallback?.Invoke();
                }
            }
        }

        [DataMember]
        public AsyncCommand RefreshCommand { get; }

        public void Cleanup()
        {
            selectedProject = null;
            Projects = null;
        }

        private async Task RefreshAsync(object? parameter, IClientContext client, CancellationToken token)
        {
            Projects = await GetProjectsAsync(client, token);

            var activeProjectName = await GetActiveProjectNameAsync(client, token);
            SelectedProject = Projects?.FirstOrDefault(t => t.Name == activeProjectName);
        }

        private async Task<ProjectModel[]> GetProjectsAsync(IClientContext client, CancellationToken token)
        {
            var result = await client.Extensibility.Workspaces().QueryProjectsAsync(
                t => t.With(t => t.Name)
                    .With(t => t.Files.Where(
                        t => t.FileName.EndsWith(".resx") || t.FileName.EndsWith(".Designer.cs"))),
                token
            );

            if (result == null)
                return [];

            var projects = result
                .Where(t => t.Files.Count > 0)
                .Select(t => new ProjectModel(t.Name, [.. t.Files.Select(t => t.Path)]))
                .OrderBy(t => t.Name);
            return [.. projects];
        }

        private async Task<string> GetActiveProjectNameAsync(IClientContext context, CancellationToken token)
        {
            var activeProject = await context.GetActiveProjectAsync(t => t.With(t => t.Name), token);
            return activeProject?.Name ?? string.Empty;
        }

        private readonly Action selectedProjectChangedCallback;
        private ProjectModel[]? projects = null;
        private ProjectModel? selectedProject = null;
    }
}
