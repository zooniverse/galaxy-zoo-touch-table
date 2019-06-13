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

        public CloseConfirmationViewModel(TableUser user)
        {
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
            GlobalData.GetInstance().Logger.AddEntry("Hide_Close_Confirmation", User.Name);
        }

        private void OnKeepClassifying(object sender)
        {
            OnToggleCloseConfirmation();
            GlobalData.GetInstance().Logger.AddEntry("Keep_Classifying", User.Name);
        }

        private void OnCloseAndEnd(object sender)
        {
            Intent = false;
            EndSession(null);
            GlobalData.GetInstance().Logger.AddEntry("Close_And_End", User.Name);
        }

        private void OnCheckIntent(object obj)
        {
            Intent = true;
            GlobalData.GetInstance().Logger.AddEntry("Intent", User.Name);
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
    }
}
