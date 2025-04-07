namespace ResxResourceExtension.Model
{
    internal class ProjectModel(string name, string[] resourceFiles) : IComparable
    {
        public string Name { get; } = name;

        public string[] ResourceFiles { get; } = resourceFiles;

        public int CompareTo(object obj) => Name.CompareTo(((ProjectModel)obj).Name);
    }
}
