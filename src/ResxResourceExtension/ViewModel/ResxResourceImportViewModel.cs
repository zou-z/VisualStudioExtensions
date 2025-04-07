using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.Shell;
using ResxResource.Model;
using ResxResource.Resource;
using ResxResourceExtension.Interface;
using ResxResourceExtension.Model;

namespace ResxResourceExtension.ViewModel
{
    internal class ResxResourceImportViewModel : ObservableObject, IResourceImport
    {
        public ResxResourceImportViewModel(Action hideImportViewAction)
        {
            this.hideImportViewAction = hideImportViewAction;
            ImportCommand = new RelayCommand(Import, () => resourceList != null && !resourceList.IsLoadingResources);
        }

        public string Text
        {
            get => text;
            set
            {
                SetProperty(ref text, value);
                TipText = string.Empty;
            }
        }

        public string TipText
        {
            get => tipText;
            set => SetProperty(ref tipText, value);
        }

        public RelayCommand ImportCommand { get; }

        public void Initialize(IResourceList resourceList)
        {
            this.resourceList = resourceList;
        }

        public async Task UpdateImportExecutableAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ImportCommand.NotifyCanExecuteChanged();
        }

        private void Import()
        {
            TipText = string.Empty;

            if (string.IsNullOrWhiteSpace(Text))
            {
                TipText = "Please enter the resources text";
                return;
            }

            if (!ResourceImporter.GetImportResourcesFromText(Text, out ResourceItem[] resources))
            {
                TipText = "The resources text is uncompleted";
                return;
            }

            if (resourceList == null)
            {
                TipText = "Internal error";
                return;
            }

            var existedResources = resourceList.GetExistedResources();
            foreach (var resource in resources)
            {
                if (existedResources?.Any(t => t.Key == resource.Key) == true)
                {
                    TipText = $"The key {resource.Key} is existed";
                    return;
                }
            }

            var resourceFiles = resourceList.GetResourceFiles();
            var modifier = resourceList.GetModifier() ?? string.Empty;
            if (!ResourceImporter.Import(resourceFiles, resources, modifier))
            {
                TipText = "Import resources failed";
                return;
            }
            resourceList.AppendResources([.. resources.Select(t => new ResourceModel(t.Key, t.NeutralText, t.EnglishText))]);

            hideImportViewAction?.Invoke();
        }

        private readonly Action hideImportViewAction;
        private IResourceList? resourceList = null;
        private string text = string.Empty;
        private string tipText = string.Empty;
    }
}
