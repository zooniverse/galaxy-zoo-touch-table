using GalaxyZooTouchTable.Utility;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class CloseConfirmationViewModel : ViewModelBase
    {
        public ICommand CheckIntent { get; set; }

        private bool _intent = false;
        public bool Intent
        {
            get => _intent;
            set => SetProperty(ref _intent, value);
        }

        public CloseConfirmationViewModel()
        {
            CheckIntent = new CustomCommand(OnCheckIntent, CanCheckIntent);
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
