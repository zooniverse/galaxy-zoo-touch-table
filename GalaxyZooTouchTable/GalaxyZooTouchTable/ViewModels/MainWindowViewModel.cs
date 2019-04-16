using System;
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
        public ClassificationPanelViewModel HeartUserVM { get; private set; }
        public ClassificationPanelViewModel EarthUserVM { get; private set; }

        private bool _dormant = true;
        public bool Dormant
        {
            get => _dormant;
            set => SetProperty(ref _dormant, value);
        }

        public MainWindowViewModel()
        {
            RegisterMessengerActions();
            PersonUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Person);
            LightUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Light);
            StarUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Star);
            HeartUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Heart);
            FaceUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Face);
            EarthUserVM = ContainerHelper.Container.Resolve<IClassificationPanelViewModelFactory>().Create(UserType.Earth);
        }

        void RegisterMessengerActions()
        {
            Messenger.Default.Register<bool>(this, OnTableActivity, "TableStateChanged");
        }

        private void OnTableActivity(bool dormant)
        {
            Dormant = dormant;
        }

        public void Load()
        {
            PersonUserVM.Load();
            FaceUserVM.Load();
            LightUserVM.Load();
            StarUserVM.Load();
            HeartUserVM.Load();
            EarthUserVM.Load();
        }
    }
}
