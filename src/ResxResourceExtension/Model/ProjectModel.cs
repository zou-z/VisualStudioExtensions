using System.Runtime.Serialization;

namespace ResxResourceExtension.Model
{
    [DataContract]
    internal class ProjectModel(string name, string[] resourceFiles): IEquatable<ProjectModel>
    {
        [DataMember]
        public string Name { get; } = name;

        public string[] ResourceFiles { get; } = resourceFiles;

        public bool Equals(ProjectModel? other)
        {
            if (other == null || Name != other.Name)
                return false;

            if (ResourceFiles.Length != other.ResourceFiles.Length)
                return false;

            var list = other.ResourceFiles.ToList();
            for (int i = 0; i < ResourceFiles.Length; ++i)
            {
                int j = 0;
                for (; j < list.Count; ++j)
                {
                    if (ResourceFiles[i] == list[j])
                        break;
                }

                if (j == list.Count)
                    return false;
                else
                    list.RemoveAt(j);
            }

            return true;
        }
    }
}
