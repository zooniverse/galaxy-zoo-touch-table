using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Services;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using Unity;
using GalaxyZooTouchTable.Models;

namespace GalaxyZooTouchTable.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IPanoptesRepository _panoptesRepository = new PanoptesRepository();
        public CenterpieceViewModel CenterpieceViewModel { get; private set; } = new CenterpieceViewModel();

        public ClassificationPanelViewModel PersonUserVM { get; private set; }
        public ClassificationPanelViewModel FaceUserVM { get; private set; }
        public ClassificationPanelViewModel LightUserVM { get; private set; }
        public ClassificationPanelViewModel StarUserVM { get; private set; }
        public ClassificationPanelViewModel HeartUserVM { get; private set; }
        public ClassificationPanelViewModel EarthUserVM { get; private set; }


        public MainWindowViewModel()
        {
            SetChildDataContext();
        }

        private void SetChildDataContext()
        {
            PersonUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Person);
            LightUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Light);
            StarUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Star);
            HeartUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Heart);
            FaceUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Face);
            EarthUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Earth);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
