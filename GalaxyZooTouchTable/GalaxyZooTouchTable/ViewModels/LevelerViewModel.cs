using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class LevelerViewModel : ViewModelBase
    {
        public TableUser User { get; set; }
        public ICommand ToggleLeveler { get; private set; }
        private const string MAX_LEVEL = "Five";
        private const int DEFAULT_CLASSIFICATIONS_UNTIL_UPGRADE = 5;
        private const int DEFAULT_CLASSIFICATIONS_COUNT = 0;
        private const string DEFAULT_CLASSIFICATION_LEVEL = "One";
        private const bool DEFAULT_CLASSIFICATION_OPEN = false;
        ClassificationPanelViewModel Classifier;

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
                if (value != 0)
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

        public LevelerViewModel(TableUser user, ClassificationPanelViewModel classifier)
        {
            Classifier = classifier;
            User = user;
            LoadCommands();
        }

        public void OnIncrementCount()
        {
            ClassificationsThisSession += 1;
        }

        private void LoadCommands()
        {
            ToggleLeveler = new CustomCommand(OnToggleLeveler);
        }

        public void CloseLeveler()
        {
            IsOpen = false;
        }

        public void OnToggleLeveler(object sender)
        {
            IsOpen = !IsOpen;
            GlobalData.GetInstance().Logger.AddEntry("Toggle_Leveler", User.Name, Classifier.CurrentSubject?.Id, Classifier.CurrentView);
        }

        public void Reset()
        {
            ClassificationsUntilUpgrade = DEFAULT_CLASSIFICATIONS_UNTIL_UPGRADE;
            ClassificationsThisSession = DEFAULT_CLASSIFICATIONS_COUNT;
            ClassificationLevel = DEFAULT_CLASSIFICATION_LEVEL;
            IsOpen = DEFAULT_CLASSIFICATION_OPEN;
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
