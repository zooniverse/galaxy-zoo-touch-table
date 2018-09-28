using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class LevelerViewModel : INotifyPropertyChanged
    {
        public TableUser User { get; set; }
        public ICommand ToggleLeveler { get; set; }

        private int _classificationsThisSession { get; set; } = 0;
        public int ClassificationsThisSession
        {
            get { return _classificationsThisSession; }
            set
            {
                if (ClassificationLevel != "Five")
                {
                    ClassificationsUntilUpgrade -= 1;
                }
                _classificationsThisSession = value;
                OnPropertyRaised("ClassificationsThisSession");
            }
        }

        private string _classificationLevel { get; set; } = "One";
        public string ClassificationLevel
        {
            get { return _classificationLevel; }
            set
            {
                _classificationLevel = value;
                OnPropertyRaised("ClassificationLevel");
            }
        }

        private int _classificationsUntilUpgrade { get; set; } = 5;
        public int ClassificationsUntilUpgrade
        {
            get { return _classificationsUntilUpgrade; }
            set
            {
                if (value == 0)
                {
                    value = 5;
                    LevelUp();
                }
                _classificationsUntilUpgrade = value;
                OnPropertyRaised("ClassificationsUntilUpgrade");
            }
        }

        private double _levelTwoOpacity { get; set; } = 0.5;
        public double LevelTwoOpacity
        {
            get { return _levelTwoOpacity; }
            set
            {
                _levelTwoOpacity = value;
                OnPropertyRaised("LevelTwoOpacity");
            }
        }

        private double _levelThreeOpacity { get; set; } = 0.5;
        public double LevelThreeOpacity
        {
            get { return _levelThreeOpacity; }
            set
            {
                _levelThreeOpacity = value;
                OnPropertyRaised("LevelThreeOpacity");
            }
        }

        private double _levelFourOpacity { get; set; } = 0.5;
        public double LevelFourOpacity
        {
            get { return _levelFourOpacity; }
            set
            {
                _levelFourOpacity = value;
                OnPropertyRaised("LevelFourOpacity");
            }
        }

        private double _levelFiveOpacity { get; set; } = 0.5;
        public double LevelFiveOpacity
        {
            get { return _levelFiveOpacity; }
            set
            {
                _levelFiveOpacity = value;
                OnPropertyRaised("LevelFiveOpacity");
            }
        }

        private bool _levelerIsOpen = false;
        public bool LevelerIsOpen
        {
            get { return _levelerIsOpen; }
            set
            {
                _levelerIsOpen = value;
                OnPropertyRaised("LevelerIsOpen");
            }
        }

        public LevelerViewModel(TableUser user = null)
        {
            User = user;
            LoadCommands();

            Messenger.Default.Register<int>(this, OnClassificationReceived);
        }

        private void OnClassificationReceived(int TotalClassifications)
        {
            Console.WriteLine(TotalClassifications);
            ClassificationsThisSession = TotalClassifications;
        }

        public void LoadCommands()
        {
            ToggleLeveler = new CustomCommand(SlideLeveler);
        }

        private void SlideLeveler(object sender)
        {
            LevelerIsOpen = !LevelerIsOpen;
        }

        private void LevelUp()
        {
            if (ClassificationsThisSession > 25)
            {
                return;
            }
            switch (ClassificationsThisSession)
            {
                case int n when (n <= 5):
                    ClassificationLevel = "Two";
                    LevelTwoOpacity = 1;
                    break;
                case int n when (n <= 10):
                    ClassificationLevel = "Three";
                    LevelThreeOpacity = 1;
                    break;
                case int n when (n <= 15):
                    ClassificationLevel = "Four";
                    LevelFourOpacity = 1;
                    break;
                case int n when (n <= 20):
                    ClassificationLevel = "Five";
                    LevelFiveOpacity = 1;
                    break;
                default:
                    ClassificationLevel = "One";
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
