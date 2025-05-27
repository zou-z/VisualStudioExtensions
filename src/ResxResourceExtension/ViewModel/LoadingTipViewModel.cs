using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;
using System.Windows;

namespace ResxResourceExtension.ViewModel
{
    [DataContract]
    internal class LoadingTipViewModel : NotifyPropertyChangedObject
    {
        private LoadingTipViewModel() { }

        public static LoadingTipViewModel Instance => instance ??= new LoadingTipViewModel();

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

        public async Task ExecuteWithTipAsync(Action action)
        {
            Show();
            await Task.Run(action);
            Hide();
        }

        public async Task<T> ExecuteWithTipAsync<T>(Func<T> action)
        {
            Show();
            var t = await Task.Run(action);
            Hide();
            return t;
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

        private static LoadingTipViewModel? instance = null;
        private bool isOperateEnabled = true;
        private Visibility loadingTextVisibility = Visibility.Collapsed;
        private string loadingText = "Loading";
    }
}
