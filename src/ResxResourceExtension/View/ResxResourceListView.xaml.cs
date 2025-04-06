using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ResxResourceExtension.View
{
    public partial class ResxResourceListView : UserControl
    {
        public ResxResourceListView()
        {
            InitializeComponent();
            Loaded += ResxResourceListView_Loaded;
        }

        private void ResxResourceListView_Loaded(object sender, RoutedEventArgs e)
        {
            textBox.Focus();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListBoxItem? listBoxItem = null;
                if (sender is DependencyObject dependencyObject)
                {
                    listBoxItem = GetParent<ListBoxItem>(dependencyObject);
                }
                listBoxItem?.Focus();
            }
        }

        private static T? GetParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = dependencyObject as DependencyObject;
            do
            {
                if (parent is T t)
                {
                    return t;
                }
                parent = VisualTreeHelper.GetParent(parent);
            } while (parent != null);
            return null;
        }
    }
}
