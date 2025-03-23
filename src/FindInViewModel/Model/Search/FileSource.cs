namespace FindInViewModel.Model.Search
{
    public class FileSource(string fromProjectName, string[] filePaths)
    {
        public string FromProjectName { get; } = fromProjectName;

        public string[] FilePaths { get; } = filePaths;
    }
}
