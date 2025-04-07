namespace ResxResourceExtension.Model
{
    internal class SolutionModel(string activeProjectName, ProjectModel[] projects)
    {
        public string ActiveProjectName { get; set; } = activeProjectName;

        public ProjectModel[] Projects { get; set; } = projects;
    }
}
