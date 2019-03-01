using GalaxyZooTouchTable.Services;
using Unity;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ViewModelLocator
    {
        IUnityContainer container = new UnityContainer();

        private static SpaceViewModel _spaceViewModel;
        public static SpaceViewModel SpaceViewModel
        {
            get => _spaceViewModel;
        }

        public ViewModelLocator()
        {
            container.RegisterType<IPanoptesService, PanoptesService>();
            _spaceViewModel = container.Resolve<SpaceViewModel>();
        }
    }
}
