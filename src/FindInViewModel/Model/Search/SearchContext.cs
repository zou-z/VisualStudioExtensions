namespace FindInViewModel.Model.Search
{
    internal class SearchContext(string fromProjectName, string targetFileName, string bindingText, FindFilesFunc findFilesFunc)
    {
        public string FromProjectName { get; set; } = fromProjectName;

        public string TargetFileName { get; } = targetFileName;

        public string BindingText { get; } = bindingText;

        public FindFilesFunc FindFilesFunc { get; } = findFilesFunc;
    }
}
