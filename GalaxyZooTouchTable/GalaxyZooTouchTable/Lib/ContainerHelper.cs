using Unity;
using Unity.Lifetime;
using GalaxyZooTouchTable.Services;

namespace GalaxyZooTouchTable.Lib
{
    public static class ContainerHelper
    {
        private static IUnityContainer _container;
        static ContainerHelper()
        {
            _container = new UnityContainer();
            _container.RegisterType<IPanoptesRepository, PanoptesRepository>(
                new ContainerControlledLifetimeManager());
        }

        public static IUnityContainer Container
        {
            get { return _container; }
        }
    }
}
