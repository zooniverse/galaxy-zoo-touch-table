using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using Unity;
using Unity.Lifetime;

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

            _container.RegisterType<IGraphQLRepository, GraphQLRepository>(
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
