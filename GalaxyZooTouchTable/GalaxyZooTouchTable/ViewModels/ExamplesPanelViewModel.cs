using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ExamplesPanelViewModel : ViewModelBase
    {
        public ICommand SelectItem { get; private set; }
        public ICommand ToggleItem { get; private set; }

        public GalaxyExample Smooth { get; set; } = GalaxyExampleFactory.Create(GalaxyType.Smooth);
        public GalaxyExample Features { get; set; } = GalaxyExampleFactory.Create(GalaxyType.Features);
        public GalaxyExample NotAGalaxy { get; set; } = GalaxyExampleFactory.Create(GalaxyType.NotAGalaxy);

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private GalaxyExample _selectedExample;
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

        public void ResetExamples()
        {
            SelectedExample = null;
        }

        private void LoadCommands()
        {
            SelectItem = new CustomCommand(OnToggleItem, CanSelectItem);
            ToggleItem = new CustomCommand(OnToggleItem);
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
    }
}
