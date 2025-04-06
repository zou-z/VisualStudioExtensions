using CommunityToolkit.Mvvm.ComponentModel;

namespace ResxResourceExtension.Model
{
    internal class ResourceModel : ObservableObject
    {
        public ResourceModel(string key, string neutralText, string englishText)
        {
            Key = LastKey = key;
            NeutralText = LastNeutralText = neutralText;
            EnglishText = LastEnglishText = englishText;
        }

        public string Key { get; set; }

        public string NeutralText { get; set; }

        public string EnglishText { get; set; }

        public string LastKey { get; private set; }

        public string LastNeutralText { get; private set; }

        public string LastEnglishText { get; private set; }

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public bool IsKeyModified => LastKey != Key;

        public bool IsNeutralTextModified => LastNeutralText != NeutralText;

        public bool IsEnglishTextModified => LastEnglishText != EnglishText;

        public void ResetModify()
        {
            LastKey = Key;
            LastNeutralText = NeutralText;
            LastEnglishText = EnglishText;
        }

        private bool isSelected = false;
    }
}
