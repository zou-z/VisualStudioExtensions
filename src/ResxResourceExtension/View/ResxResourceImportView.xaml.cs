using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ResxResourceExtension.View
{
    public partial class ResxResourceImportView : UserControl
    {
        public ResxResourceImportView()
        {
            InitializeComponent();
            IsVisibleChanged += ResxResourceImportView_IsVisibleChanged;
        }

#pragma warning disable IDE0079
        [SuppressMessage("Usage", "VSTHRD001:Avoid legacy thread switching APIs", Justification = "<Pending>")]
#pragma warning restore IDE0079
        private void ResxResourceImportView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is true)
            {
                _ = Dispatcher.InvokeAsync(() =>
                {
                    textBox.Focus();
                });
            }
        }
    }
}
