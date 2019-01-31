using System.ComponentModel;

namespace GalaxyZooTouchTable.ViewModels
{
    public interface ILevelerViewModel : INotifyPropertyChanged
    {
        void CloseLeveler();
        void LevelUp();
        void OnIncrementCount(int TotalClassifications);
        void SlideLeveler(object sender);
    }
}
