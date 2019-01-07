using GalaxyZooTouchTable.ViewModels;

namespace GalaxyZooTouchTable.Models
{
    public interface IClassificationPanelViewModelFactory
    {
        ClassificationPanelViewModel Create(UserType type);
    }
}
