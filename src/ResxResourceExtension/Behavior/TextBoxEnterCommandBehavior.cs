using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ResxResourceExtension.Behavior
{
    internal class TextBoxEnterCommandBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TextBoxEnterCommandBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(TextBoxEnterCommandBehavior), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Command?.CanExecute(CommandParameter) == true)
                {
                    Command?.Execute(CommandParameter);
                }
            }
        }
    }
}
