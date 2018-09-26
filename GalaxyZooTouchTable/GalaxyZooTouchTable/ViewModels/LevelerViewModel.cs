using System;
using GalaxyZooTouchTable.Utility;
using System.ComponentModel;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class LevelerViewModel : INotifyPropertyChanged
    {
        enum Level { One, Two, Three, Four, Five };
        public ICommand TogglePanel { get; set; }

        private bool _isOpen = false;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                OnPropertyRaised("IsOpen");
            }
        }

        public LevelerViewModel()
        {
            TogglePanel = new CustomCommand(SlidePanel);
        }

        private void SlidePanel(object sender)
        {
            IsOpen = !IsOpen;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
