using ResxResourceExtension.Model;

namespace ResxResourceExtension.Interface
{
    internal interface IResourceList
    {
        bool IsLoading { get; set; }

        ResourceModel[] GetExistedResources();

        string[] GetResourceFiles();

        string GetModifier();

        void AppendResources(ResourceModel[] resources);
    }
}
