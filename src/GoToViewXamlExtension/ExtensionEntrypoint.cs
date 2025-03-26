using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

namespace GoToViewXamlExtension
{
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Extension
    {
        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            Metadata = new(
                    id: "GoToViewXamlExtension.d855ca86-af27-47e1-9f1b-f481534b0328",
                    version: this.ExtensionAssemblyVersion,
                    publisherName: "Publisher name",
                    displayName: "GoToViewXamlExtension",
                    description: "Extension description"),
        };

        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            base.InitializeServices(serviceCollection);
        }
    }
}
