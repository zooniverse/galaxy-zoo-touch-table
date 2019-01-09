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

        private int _classificationsUntilUpgrade = 5;
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

        private int _classificationsThisSession = 0;
        public int ClassificationsThisSession
        {
            get => _classificationsThisSession;
            set
            {
                ClassificationsUntilUpgrade--;
                SetProperty(ref _classificationsThisSession, value);
            }
        }

        private string _classificationLevel = "One";
        public string ClassificationLevel
        {
            get => _classificationLevel;
            set => SetProperty(ref _classificationLevel, value);
        }

        private bool _isOpen = false;
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
