using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class LevelerViewModel : ViewModelBase
    {
        public TableUser User { get; set; }
        public ICommand ToggleLeveler { get; private set; }
        const string MAX_LEVEL = "Five";
        const int DEFAULT_CLASSIFICATIONS_UNTIL_UPGRADE = 5;
        const int DEFAULT_CLASSIFICATIONS_COUNT = 5;
        const string DEFAULT_CLASSIFICATION_LEVEL = "One";
        const bool DEFAULT_CLASSIFICATION_OPEN = false;

        private int _classificationsUntilUpgrade = DEFAULT_CLASSIFICATIONS_UNTIL_UPGRADE;
        public int ClassificationsUntilUpgrade
        {
            get => _classificationsUntilUpgrade;
            set
            {
                if (value <= 0)
                {
                    value = 5;
                    LevelUp();
                }
                SetProperty(ref _classificationsUntilUpgrade, value);
            }
        }

        private int _classificationsThisSession = DEFAULT_CLASSIFICATIONS_COUNT;
        public int ClassificationsThisSession
        {
            get => _classificationsThisSession;
            set
            {
                ClassificationsUntilUpgrade--;
                SetProperty(ref _classificationsThisSession, value);
            }
        }

        private string _classificationLevel = DEFAULT_CLASSIFICATION_LEVEL;
        public string ClassificationLevel
        {
            get => _classificationLevel;
            set => SetProperty(ref _classificationLevel, value);
        }

        private bool _isOpen = DEFAULT_CLASSIFICATION_OPEN;
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        public LevelerViewModel(TableUser user)
        {
            User = user;
            LoadCommands();
        }

        public void OnIncrementCount(int TotalClassifications)
        {
            ClassificationsThisSession = TotalClassifications;
        }

        public void LoadCommands()
        {
            ToggleLeveler = new CustomCommand(SlideLeveler);
        }

        private void SlideLeveler(object sender)
        {
            IsOpen = !IsOpen;
        }

        private void LevelUp()
        {
            if (ClassificationLevel == MAX_LEVEL)
            {
                return;
            }
            switch (ClassificationsThisSession)
            {
                case int n when (n <= 6):
                    ClassificationLevel = "Two";
                    break;
                case int n when (n <= 12):
                    ClassificationLevel = "Three";
                    break;
                case int n when (n <= 18):
                    ClassificationLevel = "Four";
                    break;
                case int n when (n <= 24):
                    ClassificationLevel = MAX_LEVEL;
                    break;
                default:
                    ClassificationLevel = "One";
                    break;
            }
        }
    }
}
