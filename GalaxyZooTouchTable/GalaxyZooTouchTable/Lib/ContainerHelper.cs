using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using Unity;

namespace GalaxyZooTouchTable.Lib
{
    public static class ContainerHelper
    {
        private static IUnityContainer _container;
        static ContainerHelper()
        {
            _container = new UnityContainer();
            _container.RegisterType<IPanoptesService, PanoptesService>();

            _container.RegisterType<IGraphQLService, GraphQLService>(
                new ContainerControlledLifetimeManager());

            _container.RegisterType<IClassificationPanelViewModelFactory, ClassificationPanelViewModelFactory>(
                new ContainerControlledLifetimeManager());
        }

        public static IUnityContainer Container
        {
            get { return _container ?? new UnityContainer(); }
        }
    }
}
