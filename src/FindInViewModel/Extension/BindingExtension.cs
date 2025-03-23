using System.IO;

namespace FindInViewModel.Extension
{
    internal static class BindingExtension
    {
        public static bool IsCommandBinding(this string bindingText)
        {
            return bindingText.EndsWith(commandText);
        }

        public static string GetBindingMethodName(this string bindingText)
        {
            return bindingText.Substring(0, bindingText.Length - commandText.Length);
        }

        public static string GetViewModelName(this string viewFileName)
        {
            return $"{Path.GetFileNameWithoutExtension(viewFileName)}{viewModelText}";
        }

        private readonly static string commandText = "Command";
        private readonly static string viewModelText = "ViewModel";
    }
}
