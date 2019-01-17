using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.ViewModels;
using System;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationPanelViewModelFactory : IClassificationPanelViewModelFactory
    {
        private IPanoptesService _panoptesRepo;
        private IGraphQLService _graphQLRepo;

        public ClassificationPanelViewModelFactory(IPanoptesService panoptesRepo, IGraphQLService graphQLRepo)
        {
            if (panoptesRepo == null || graphQLRepo == null)
            {
                throw new ArgumentNullException("RepoDependency");
            }

            _panoptesRepo = panoptesRepo;
            _graphQLRepo = graphQLRepo;
        }

        public ClassificationPanelViewModel Create(UserType type)
        {
            switch (type)
            {
                case UserType.Person:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, GlobalData.GetInstance().PersonUser);
                case UserType.Star:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, GlobalData.GetInstance().StarUser);
                case UserType.Earth:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, GlobalData.GetInstance().EarthUser);
                case UserType.Light:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, GlobalData.GetInstance().LightUser);
                case UserType.Face:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, GlobalData.GetInstance().FaceUser);
                case UserType.Heart:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, GlobalData.GetInstance().HeartUser);
                default:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, GlobalData.GetInstance().HeartUser);
            }
        }
    }
}
