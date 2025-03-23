namespace FindInViewModel.Model.Search
{
    internal class SearchResult(string fromProjectName, string variableTypeName, FilePosition filePosition)
    {
        public string FromProjectName { get; } = fromProjectName;

        public string VariableTypeName { get; } = variableTypeName;

        public FilePosition FilePosition { get; } = filePosition;
    }
}
