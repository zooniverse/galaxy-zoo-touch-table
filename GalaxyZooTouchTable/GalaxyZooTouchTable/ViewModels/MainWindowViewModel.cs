using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using Unity;

namespace GalaxyZooTouchTable.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ClassificationPanelViewModel PurpleUserVM { get; private set; }
        public ClassificationPanelViewModel PeachUserVM { get; private set; }
        public ClassificationPanelViewModel AquaUserVM { get; private set; }
        public ClassificationPanelViewModel BlueUserVM { get; private set; }
        public ClassificationPanelViewModel PinkUserVM { get; private set; }
        public ClassificationPanelViewModel GreenUserVM { get; private set; }

        private bool _isDormant = true;
        public bool IsDormant
        {
            get => _isDormant;
            set => SetProperty(ref _isDormant, value);
        }

        public MainWindowViewModel()
        {
            RegisterMessengerActions();
            PurpleUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Purple);
            AquaUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Aqua);
            BlueUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Blue);
            PinkUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Pink);
            PeachUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Peach);
            GreenUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Green);
        }

        void RegisterMessengerActions()
        {
            Messenger.Default.Register<bool>(this, OnTableActivity, "TableStateChanged");
        }

        void OnTableActivity(bool dormant) => IsDormant = dormant;

        public void Load()
        {
            PurpleUserVM.Load();
            PeachUserVM.Load();
            AquaUserVM.Load();
            BlueUserVM.Load();
            PinkUserVM.Load();
            GreenUserVM.Load();
        }
    }
}
