using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.Extensibility.UI;
using ResxResource.Resource;
using System.Runtime.Serialization;

namespace ResxResourceExtension.ViewModel
{
    [DataContract]
    internal class ImportViewModel : NotifyPropertyChangedObject
    {
        public ImportViewModel(
            Func<bool> getIsSelectProjectCallback,
            Func<string> getModifierCallback,
            Func<string[]> getResourceFilesCallback)
        {
            this.getIsSelectProjectCallback = getIsSelectProjectCallback;
            this.getModifierCallback = getModifierCallback;
            this.getResourceFilesCallback = getResourceFilesCallback;
            ImportCommand = new AsyncCommand(ImportAsync);
        }

        [DataMember]
        public string ImportText
        {
            get => importText;
            set => SetProperty(ref importText, value);
        }

        [DataMember]
        public AsyncCommand ImportCommand { get; }

        public void Cleanup()
        {
            ImportText = string.Empty;
        }

        private async Task ImportAsync(object? parameter, IClientContext context, CancellationToken token)
        {
            await ImportAsync(context, token);
        }

        private async Task ImportAsync(IClientContext context, CancellationToken token)
        {
            if (string.IsNullOrEmpty(ImportText))
            {
                await context.Extensibility.Shell().ShowPromptAsync("Please enter the resources text.", PromptOptions.OK, token);
                return;
            }

            if (!getIsSelectProjectCallback())
            {
                await context.Extensibility.Shell().ShowPromptAsync("Please select project first.", PromptOptions.OK, token);
                return;
            }

            if (!ResourceImporter.GetImportResourcesFromText(ImportText, out var resources))
            {
                await context.Extensibility.Shell().ShowPromptAsync("The resources text is uncompleted.", PromptOptions.OK, token);
                return;
            }

            var resourceFiles = getResourceFilesCallback();
            var modifier = getModifierCallback();
            modifier = string.IsNullOrEmpty(modifier) ? "public" : modifier;

            if (!ResourceImporter.Import(resourceFiles, resources, modifier))
            {
                await context.Extensibility.Shell().ShowPromptAsync("Import resources failed.", PromptOptions.OK, token);
                return;
            }

            ImportText = string.Empty;
        }

        private readonly Func<bool> getIsSelectProjectCallback;
        private readonly Func<string> getModifierCallback;
        private readonly Func<string[]> getResourceFilesCallback;
        private string importText = string.Empty;
    }
}
