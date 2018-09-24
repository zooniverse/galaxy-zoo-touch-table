using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ExamplesPanelViewModel : INotifyPropertyChanged
    {
        public List<GalaxyExample> ExampleGalaxies { get; set; } = new List<GalaxyExample>();

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

        public ICommand OpenPanel { get; set; }
        public ICommand TogglePanel { get; set; }
        public ICommand SelectionChanged { get; set; }

        public bool _isOpen = false;
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
            ExampleGalaxies.Add(GalaxyExampleFactory.Create(GalaxyType.Smooth));
            ExampleGalaxies.Add(GalaxyExampleFactory.Create(GalaxyType.Features));
            ExampleGalaxies.Add(GalaxyExampleFactory.Create(GalaxyType.Star));

            LoadCommands();
        }

        public void LoadCommands()
        {
            OpenPanel = new CustomCommand(SlidePanel, CanOpen);
            TogglePanel = new CustomCommand(SlidePanel, CanToggle);
            SelectionChanged = new CustomCommand(SelectExample, CanChoose);
        }

        public void SelectExample(object sender)
        {
            if (sender == null)
            {
                IsSelected = false;
            } else
            {
                IsSelected = true;
            }
        }

        public bool CanChoose(object sender)
        {
            return true;
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
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

    }
}
