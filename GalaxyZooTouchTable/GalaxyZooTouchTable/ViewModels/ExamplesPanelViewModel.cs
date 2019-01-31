using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ExamplesPanelViewModel : ViewModelBase, IExamplesPanelViewModel
    {
        public ICommand OpenPanel { get; private set; }
        public ICommand TogglePanel { get; private set; }
        public ICommand SelectItem { get; private set; }
        public ICommand UnselectItem { get; private set; }

        public GalaxyExample Smooth { get; set; } = GalaxyExampleFactory.Create(GalaxyType.Smooth);
        public GalaxyExample Features { get; set; } = GalaxyExampleFactory.Create(GalaxyType.Features);
        public GalaxyExample NotAGalaxy { get; set; } = GalaxyExampleFactory.Create(GalaxyType.NotAGalaxy);

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool _isOpen = true;
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        public GalaxyExample _selectedExample;
        public GalaxyExample SelectedExample
        {
            get => _selectedExample;
            set
            {
                IsSelected = value == null ? false : true;
                SetProperty(ref _selectedExample, value);
            }
        }

        public ExamplesPanelViewModel()
        {
            LoadCommands();
        }

        private void LoadCommands()
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
                SelectedExample = null;
            } else
            {
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
    }
}
