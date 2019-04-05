using GalaxyZooTouchTable.ViewModels;

namespace GalaxyZooTouchTable.Models
{
    public class NotificationAvatar : ViewModelBase
    {
        private bool _showCircle = false;
        public bool ShowCircle
        {
            get => _showCircle;
            set => SetProperty(ref _showCircle, value);
        }

        private bool _showExclamationPoint = false;
        public bool ShowExclamationPoint
        {
            get => _showExclamationPoint;
            set => SetProperty(ref _showExclamationPoint, value);
        }

        private bool _showQuestion = false;
        public bool ShowQuestion
        {
            get => _showQuestion;
            set => SetProperty(ref _showQuestion, value);
        }

        private bool _disabled = false;
        public bool Disabled
        {
            get => _disabled;
            set => SetProperty(ref _disabled, value);
        }
    }
}
