namespace ResxResource.Model
{
    public class ResourceItem(string key, string neutralText, string englishText)
    {
        public string Key { get; set; } = key;

        public string NeutralText { get; set; } = neutralText;

        public string EnglishText { get; set; } = englishText;
    }
}
