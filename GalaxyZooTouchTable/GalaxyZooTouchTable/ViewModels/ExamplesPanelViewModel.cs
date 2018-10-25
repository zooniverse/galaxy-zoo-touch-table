using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System.ComponentModel;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ExamplesPanelViewModel : INotifyPropertyChanged
    {
        public ICommand OpenPanel { get; set; }
        public ICommand TogglePanel { get; set; }
        public ICommand SelectItem { get; set; }
        public ICommand UnselectItem { get; set; }

        public GalaxyExample Smooth { get; set; } = GalaxyExampleFactory.Create(GalaxyType.Smooth);
        public GalaxyExample Features { get; set; } = GalaxyExampleFactory.Create(GalaxyType.Features);
        public GalaxyExample NotAGalaxy { get; set; } = GalaxyExampleFactory.Create(GalaxyType.NotAGalaxy);

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyRaised("IsSelected");
            }
        }

        public bool _isOpen = true;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                OnPropertyRaised("IsOpen");
            }
        }

        public GalaxyExample _selectedExample;
        public GalaxyExample SelectedExample
        {
            get { return _selectedExample; }
            set
            {
                _selectedExample = value;
                OnPropertyRaised("SelectedExample");
            }
        }

        public ExamplesPanelViewModel()
        {
            LoadCommands();
        }

        public void LoadCommands()
        {
            OpenPanel = new CustomCommand(SlidePanel, CanOpen);
            TogglePanel = new CustomCommand(SlidePanel, CanToggle);
            SelectItem = new CustomCommand(OnToggleItem, CanSelectItem);
            UnselectItem = new CustomCommand(OnToggleItem);
        }

        public void OnToggleItem(object sender)
        {
            var example = sender as GalaxyExample;
            if (example == SelectedExample)
            {
                IsSelected = false;
                SelectedExample = null;
            } else
            {
                IsSelected = true;
                SelectedExample = example;
            }
        }

        public bool CanSelectItem(object sender)
        {
            return SelectedExample == null;
        }

        public void SlidePanel(object sender)
        {
            IsOpen = !IsOpen;
        }

        public bool CanOpen(object sender)
        {
            return !IsOpen;
        }

        public bool CanToggle(object sender)
        {
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

    }
}
