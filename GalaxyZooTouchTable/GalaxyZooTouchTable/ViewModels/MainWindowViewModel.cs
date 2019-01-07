using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using System.ComponentModel;

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
            PersonUserVM = new ClassificationPanelViewModel(_panoptesRepository, CommonData.GetInstance().PersonUser);
            LightUserVM = new ClassificationPanelViewModel(_panoptesRepository, CommonData.GetInstance().LightUser);
            StarUserVM = new ClassificationPanelViewModel(_panoptesRepository, CommonData.GetInstance().StarUser);
            HeartUserVM = new ClassificationPanelViewModel(_panoptesRepository, CommonData.GetInstance().HeartUser);
            FaceUserVM = new ClassificationPanelViewModel(_panoptesRepository, CommonData.GetInstance().FaceUser);
            EarthUserVM = new ClassificationPanelViewModel(_panoptesRepository, CommonData.GetInstance().EarthUser);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
