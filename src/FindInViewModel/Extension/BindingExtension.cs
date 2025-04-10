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
            var name = Path.GetFileNameWithoutExtension(viewFileName);
            name = name.EndsWith("View") ? name.Substring(0, name.Length - 4) : name;
            return $"{name}{viewModelText}";
        }

        private readonly static string commandText = "Command";
        private readonly static string viewModelText = "ViewModel";
    }
}
