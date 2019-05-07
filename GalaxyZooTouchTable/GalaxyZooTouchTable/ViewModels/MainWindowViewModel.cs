using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using Unity;

namespace GalaxyZooTouchTable.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ClassificationPanelViewModel PersonUserVM { get; private set; }
        public ClassificationPanelViewModel FaceUserVM { get; private set; }
        public ClassificationPanelViewModel LightUserVM { get; private set; }
        public ClassificationPanelViewModel StarUserVM { get; private set; }
        public ClassificationPanelViewModel PinkUserVM { get; private set; }
        public ClassificationPanelViewModel EarthUserVM { get; private set; }

        private bool _isDormant = true;
        public bool IsDormant
        {
            get => _isDormant;
            set => SetProperty(ref _isDormant, value);
        }

        public MainWindowViewModel()
        {
            RegisterMessengerActions();
            PersonUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Person);
            LightUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Light);
            StarUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Star);
            PinkUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Pink);
            FaceUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Face);
            EarthUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Earth);
        }

        void RegisterMessengerActions()
        {
            Messenger.Default.Register<bool>(this, OnTableActivity, "TableStateChanged");
        }

        void OnTableActivity(bool dormant) => IsDormant = dormant;

        public void Load()
        {
            PersonUserVM.Load();
            FaceUserVM.Load();
            LightUserVM.Load();
            StarUserVM.Load();
            PinkUserVM.Load();
            EarthUserVM.Load();
        }
    }
}
