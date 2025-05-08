using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;

namespace ResxResourceExtension.Model
{
    [DataContract]
    internal class ResourceModel : NotifyPropertyChangedObject
    {
        public ResourceModel(string key, string neutralText, string englishText)
        {
            Key = LastKey = key;
            NeutralText = LastNeutralText = neutralText;
            EnglishText = LastEnglishText = englishText;
        }

        [DataMember]
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        [DataMember]
        public string Key
        {
            get => key;
            set
            {
                if (SetProperty(ref key, value))
                {
                    RaiseNotifyPropertyChangedEvent(nameof(IsKeyModified));
                }
            }
        }

        [DataMember]
        public string NeutralText
        {
            get => neutralText;
            set
            {
                if (SetProperty(ref neutralText, value))
                {
                    RaiseNotifyPropertyChangedEvent(nameof(IsNeutralTextModified));
                }
            }
        }

        [DataMember]
        public string EnglishText
        {
            get => englishText;
            set
            {
                if (SetProperty(ref englishText, value))
                {
                    RaiseNotifyPropertyChangedEvent(nameof(IsEnglishTextModified));
                }
            }
        }

        [DataMember]
        public bool IsKeyModified => LastKey != Key;

        [DataMember]
        public bool IsNeutralTextModified => LastNeutralText != NeutralText;

        [DataMember]
        public bool IsEnglishTextModified => LastEnglishText != EnglishText;

        public string LastKey { get; private set; }

        public string LastNeutralText { get; private set; }

        public string LastEnglishText { get; private set; }

        public void ResetModifyStatus()
        {
            LastKey = Key;
            LastNeutralText = NeutralText;
            LastEnglishText = EnglishText;
            RaiseNotifyPropertyChangedEvent(nameof(IsKeyModified));
            RaiseNotifyPropertyChangedEvent(nameof(IsNeutralTextModified));
            RaiseNotifyPropertyChangedEvent(nameof(IsEnglishTextModified));
        }

        private bool isSelected = false;
        private string key = string.Empty;
        private string neutralText = string.Empty;
        private string englishText = string.Empty;
    }
}
