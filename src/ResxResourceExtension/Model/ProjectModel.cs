namespace ResxResourceExtension.Model
{
    internal class ProjectModel(string name, string[] resourceFiles)
    {
        public string Name { get; } = name;

        public string[] ResourceFiles { get; } = resourceFiles;
    }
}
