using System.Windows;

namespace ResxResourceExtension.View
{
    internal class SingletonWindow : Window
    {
        protected SingletonWindow()
        {
            Closed += SingletonWindow_Closed;
        }

        protected static SingletonWindow Create(string title, double width, double height, Action? onWindowClosed = null)
        {
            SingletonWindow.onWindowClosed = onWindowClosed;
            return instance ??= new SingletonWindow()
            {
                Title = title,
                Width = width,
                Height = height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
        }

        private void SingletonWindow_Closed(object sender, EventArgs e)
        {
            if (instance != null)
            {
                onWindowClosed?.Invoke();
                onWindowClosed = null;

                instance.Closed += SingletonWindow_Closed;
                instance.DataContext = null;
                instance.Content = null;
                instance = null;
            }
        }

        private static SingletonWindow? instance = null;
        private static Action? onWindowClosed = null;
    }
}
