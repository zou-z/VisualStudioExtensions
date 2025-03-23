namespace FindInViewModel.Model.Search
{
    public class FilePosition(string filePath, int lineIndex, int columnIndex, int columnLength)
    {
        public string FilePath { get; } = filePath;

        public int LineIndex { get; } = lineIndex;

        public int ColumnIndex { get; } = columnIndex;

        public int ColumnLength { get; } = columnLength;

        public static FilePosition Empty => emptyFilePosition ??= new(string.Empty, 0, 0, 0);

        private static FilePosition? emptyFilePosition = null;
    }
}
