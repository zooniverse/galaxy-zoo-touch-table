using Unity;
using Unity.Lifetime;
using GalaxyZooTouchTable.Services;
using Microsoft.Practices.Unity;
using GalaxyZooTouchTable.Models;
using Unity.Injection;

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

            _container.RegisterType<IClassificationPanelViewModelFactory, ClassificationPanelViewModelFactory>(
                new ContainerControlledLifetimeManager());
        }

        public static IUnityContainer Container
        {
            get { return _container; }
        }
    }
}
