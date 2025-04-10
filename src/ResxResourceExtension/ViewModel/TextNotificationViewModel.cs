using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ResxResourceExtension.ViewModel
{
    internal class TextNotificationViewModel : ObservableObject
    {
        public enum Severity
        {
            Informational,
            Success,
            Warning,
            Error,
        }

        public TextNotificationViewModel()
        {
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public Brush ColorBrush
        {
            get => colorBrush;
            set => SetProperty(ref colorBrush, value);
        }

        public Visibility Visibility
        {
            get => visibility;
            set => SetProperty(ref visibility, value);
        }

        public void Show(string message, Severity severity)
        {
            StopTimer();
            InitParameters(message, severity);
            StartTimer();
        }

        public void Close()
        {
            StopTimer();
            Visibility = Visibility.Collapsed;
        }

        private void InitParameters(string message, Severity severity)
        {
            Text = message;
            ColorBrush = severity switch
            {
                Severity.Informational => Brushes.White,
                Severity.Success => Brushes.Green,
                Severity.Warning => Brushes.Yellow,
                Severity.Error => Brushes.Red,
                _ => Brushes.White,
            };
        }

        private void StartTimer()
        {
            Visibility = Visibility.Visible;
            if (timer == null)
            {
                timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(delayMilliseconds) };
                timer.Tick += (sender, e) =>
                {
                    Visibility = Visibility.Collapsed;
                };
            }
            timer.Start();
        }

        private void StopTimer()
        {
            timer?.Stop();
        }

        private string text=string.Empty;
        private Brush colorBrush = Brushes.White;
        private Visibility visibility = Visibility.Collapsed;
        private DispatcherTimer? timer = null;
        private const int delayMilliseconds = 2000;
    }
}
