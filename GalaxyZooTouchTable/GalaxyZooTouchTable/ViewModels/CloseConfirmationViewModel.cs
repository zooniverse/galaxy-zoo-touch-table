using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class CloseConfirmationViewModel : ViewModelBase
    {
        public event Action CheckOverlay = delegate { };
        public event Action<object> EndSession = delegate { };

        public ICommand CheckIntent { get; private set; }
        public ICommand CloseAndEnd { get; private set; }
        public ICommand ToggleCloseConfirmation { get; private set; }
        public ICommand HideCloseConfirmation { get; private set; }
        public ICommand KeepClassifying { get; private set; }
        ClassificationPanelViewModel Classifier;
        TableUser User { get; set; }

        private bool _intent;
        public bool Intent
        {
            get => _intent;
            set => SetProperty(ref _intent, value);
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                SetProperty(ref _isVisible, value);
                CheckOverlay();
            }
        }

        public CloseConfirmationViewModel(TableUser user, ClassificationPanelViewModel classifier)
        {
            Classifier = classifier;
            User = user;
            CheckIntent = new CustomCommand(OnCheckIntent, CanCheckIntent);
            ToggleCloseConfirmation = new CustomCommand(OnToggleCloseConfirmation);
            HideCloseConfirmation = new CustomCommand(OnHideCloseConfirmation);
            KeepClassifying = new CustomCommand(OnKeepClassifying);
            CloseAndEnd = new CustomCommand(OnCloseAndEnd);
        }

        private void OnHideCloseConfirmation(object sender)
        {
            OnToggleCloseConfirmation();
            LogEvent("Hide_Close_Confirmation");
        }

        private void OnKeepClassifying(object sender)
        {
            OnToggleCloseConfirmation();
            LogEvent("Keep_Classifying");
        }

        private void OnCloseAndEnd(object sender)
        {
            Intent = false;
            EndSession(null);
            LogEvent("Close_And_End");
        }

        private void OnCheckIntent(object obj)
        {
            Intent = true;
            LogEvent("Intent");
        }

        public void OnToggleCloseConfirmation(object visible = null)
        {
            bool? show = visible as bool?;
            IsVisible = show ?? !IsVisible;
        }

        private bool CanCheckIntent(object obj)
        {
            return !Intent;
        }

        void LogEvent(string entry)
        {
            GlobalData.GetInstance().Logger.AddEntry(entry, User.Name, Classifier.CurrentSubject?.Id, Classifier.CurrentView);
        }
    }
}
