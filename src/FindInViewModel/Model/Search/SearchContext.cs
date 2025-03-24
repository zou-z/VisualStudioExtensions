using System.Threading;

namespace FindInViewModel.Model.Search
{
    internal class SearchContext(
        string fromProjectName,
        string targetFileName,
        string bindingText,
        FindFilesAsyncFunc findFilesAsyncFunc,
        CancellationToken cancellationToken)
    {
        public string FromProjectName { get; set; } = fromProjectName;

        public string TargetFileName { get; } = targetFileName;

        public string BindingText { get; } = bindingText;

        public FindFilesAsyncFunc FindFilesAsyncFunc { get; } = findFilesAsyncFunc;

        public CancellationToken CancellationToken { get; } = cancellationToken;
    }
}
