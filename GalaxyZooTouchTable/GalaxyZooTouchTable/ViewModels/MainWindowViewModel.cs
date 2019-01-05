using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using System.ComponentModel;

namespace GalaxyZooTouchTable.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IPanoptesRepository _panoptesRepository = new PanoptesRepository();

        private ClassificationPanelViewModel _personUserVM;
        public ClassificationPanelViewModel PersonUserVM
        {
            get { return _personUserVM; }
            set
            {
                _personUserVM = value;
                OnPropertyRaised("PersonUserVM");
            }
        }

        private ClassificationPanelViewModel _faceUserVM;
        public ClassificationPanelViewModel FaceUserVM
        {
            get { return _faceUserVM; }
            set
            {
                _faceUserVM = value;
                OnPropertyRaised("FaceUserVM");
            }
        }

        private ClassificationPanelViewModel _lightUserVM;
        public ClassificationPanelViewModel LightUserVM
        {
            get { return _lightUserVM; }
            set
            {
                _lightUserVM = value;
                OnPropertyRaised("LightUserVM");
            }
        }

        private ClassificationPanelViewModel _starUserVM;
        public ClassificationPanelViewModel StarUserVM
        {
            get { return _starUserVM; }
            set
            {
                _starUserVM = value;
                OnPropertyRaised("StarUserVM");
            }
        }

        private ClassificationPanelViewModel _heartUserVM;
        public ClassificationPanelViewModel HeartUserVM
        {
            get { return _heartUserVM; }
            set
            {
                _heartUserVM = value;
                OnPropertyRaised("HeartUserVM");
            }
        }

        private ClassificationPanelViewModel _earthUserVM;
        public ClassificationPanelViewModel EarthUserVM
        {
            get { return _earthUserVM; }
            set
            {
                _earthUserVM = value;
                OnPropertyRaised("EarthUserVM");
            }
        }

        public CenterpieceViewModel CenterpieceViewModel { get; set; }

        public MainWindowViewModel()
        {
            SetChildDataContext();
            CenterpieceViewModel = new CenterpieceViewModel();
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
