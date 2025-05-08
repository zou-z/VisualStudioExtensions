using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;
using System.Windows;

namespace ResxResourceExtension.ViewModel
{
    [DataContract]
    internal class LoadingTipViewModel : NotifyPropertyChangedObject
    {
        [DataMember]
        public bool IsOperateEnabled
        {
            get => isOperateEnabled;
            set => SetProperty(ref isOperateEnabled, value);
        }

        [DataMember]
        public Visibility LoadingTextVisibility
        {
            get => loadingTextVisibility;
            set => SetProperty(ref loadingTextVisibility, value);
        }

        [DataMember]
        public string LoadingText
        {
            get => loadingText;
            set => SetProperty(ref loadingText, value);
        }

        public void Show()
        {
            IsOperateEnabled = false;
            LoadingTextVisibility = Visibility.Visible;
        }

        public void Hide()
        {
            IsOperateEnabled = true;
            LoadingTextVisibility = Visibility.Collapsed;
        }

        private bool isOperateEnabled = true;
        private Visibility loadingTextVisibility = Visibility.Collapsed;
        private string loadingText = "Loading";
    }
}
