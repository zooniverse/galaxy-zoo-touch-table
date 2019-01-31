using System.ComponentModel;

namespace GalaxyZooTouchTable.ViewModels
{
    public interface IExamplesPanelViewModel : INotifyPropertyChanged
    {
        bool CanOpen(object sender);
        bool CanSelectItem(object sender);
        bool CanToggle(object sender);
        void OnToggleItem(object sender);
        void ResetExamples();
        void SlidePanel(object sender);
    }
}
