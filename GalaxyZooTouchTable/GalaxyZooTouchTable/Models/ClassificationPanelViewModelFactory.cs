using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.ViewModels;
using System;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationPanelViewModelFactory : IClassificationPanelViewModelFactory
    {
        private IPanoptesRepository _panoptesRepo;
        private IGraphQLRepository _graphQLRepo;

        public ClassificationPanelViewModelFactory(IPanoptesRepository panoptesRepo, IGraphQLRepository graphQLRepo)
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
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, CommonData.GetInstance().PersonUser);
                case UserType.Star:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, CommonData.GetInstance().StarUser);
                case UserType.Earth:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, CommonData.GetInstance().EarthUser);
                case UserType.Light:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, CommonData.GetInstance().LightUser);
                case UserType.Face:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, CommonData.GetInstance().FaceUser);
                case UserType.Heart:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, CommonData.GetInstance().HeartUser);
                default:
                    return new ClassificationPanelViewModel(_panoptesRepo, _graphQLRepo, CommonData.GetInstance().HeartUser);
            }
        }
    }
}
