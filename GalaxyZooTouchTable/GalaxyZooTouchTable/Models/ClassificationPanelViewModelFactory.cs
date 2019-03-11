using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.ViewModels;
using System;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationPanelViewModelFactory : IClassificationPanelViewModelFactory
    {
        private ILocalDBService _localDBService;
        private IGraphQLService _graphQLRepo;
        private IPanoptesService _panoptesRepo;

        public ClassificationPanelViewModelFactory(IPanoptesService panoptesRepo, IGraphQLService graphQLRepo, ILocalDBService localDBService)
        {
            if (panoptesRepo == null || graphQLRepo == null || localDBService == null)
            {
                throw new ArgumentNullException("RepoDependency");
            }

            _localDBService = localDBService;
            _graphQLRepo = graphQLRepo; 
            _panoptesRepo = panoptesRepo;
        }

        public ClassificationPanelViewModel Create(UserType type)
        {
            switch (type)
            {
                case UserType.Person:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, _localDBService, GlobalData.GetInstance().PersonUser);
                case UserType.Star:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, _localDBService, GlobalData.GetInstance().StarUser);
                case UserType.Earth:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, _localDBService, GlobalData.GetInstance().EarthUser);
                case UserType.Light:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, _localDBService, GlobalData.GetInstance().LightUser);
                case UserType.Face:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, _localDBService, GlobalData.GetInstance().FaceUser);
                case UserType.Heart:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, _localDBService, GlobalData.GetInstance().HeartUser);
                default:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, _localDBService, GlobalData.GetInstance().HeartUser);
            }
        }
    }
}
