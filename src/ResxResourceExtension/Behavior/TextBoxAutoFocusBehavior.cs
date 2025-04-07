using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace ResxResourceExtension.Behavior
{
    internal class TextBoxAutoFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.IsEnabledChanged += AssociatedObject_IsEnabledChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.IsEnabledChanged -= AssociatedObject_IsEnabledChanged;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Focus();
        }

        private void AssociatedObject_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AssociatedObject.Focus();
        }
    }
}
