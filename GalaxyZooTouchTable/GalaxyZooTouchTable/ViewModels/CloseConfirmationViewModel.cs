using GalaxyZooTouchTable.Utility;
using System;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class CloseConfirmationViewModel : ViewModelBase
    {
        public event Action<object> EndSession = delegate { };
        public ICommand CloseClassifier { get; private set; }
        public ICommand CheckIntent { get; private set; }
        public ICommand ToggleCloseConfirmation { get; private set; }

        private bool _intent = false;
        public bool Intent
        {
            get => _intent;
            set => SetProperty(ref _intent, value);
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public CloseConfirmationViewModel()
        {
            CheckIntent = new CustomCommand(OnCheckIntent, CanCheckIntent);
            CloseClassifier = new CustomCommand(OnCloseClassifier);
            ToggleCloseConfirmation = new CustomCommand(OnToggleCloseConfirmation);
        }

        private void OnCloseClassifier(object obj)
        {
            Intent = false;
            EndSession(null);
        }

        public void OnToggleCloseConfirmation(object sender = null)
        {
            IsVisible = !IsVisible;
        }

        private bool CanCheckIntent(object obj)
        {
            return !Intent;
        }

        private void OnCheckIntent(object obj)
        {
            Intent = true;
        }
    }
}
