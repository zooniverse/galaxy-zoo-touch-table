namespace GalaxyZooTouchTable.ViewModels
{
    interface IExamplesPanelViewModel
    {
        bool CanOpen(object sender);
        bool CanSelectItem(object sender);
        bool CanToggle(object sender);
        void OnToggleItem(object sender);
        void SlidePanel(object sender);
    }
}
