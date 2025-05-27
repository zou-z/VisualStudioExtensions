using System.Runtime.Serialization;

namespace ResxResourceExtension.Model
{
    [DataContract]
    internal class ProjectModel(string name, string[] resourceFiles)
    {
        [DataMember]
        public string Name { get; } = name;

        public string[] ResourceFiles { get; } = resourceFiles;
    }
}
