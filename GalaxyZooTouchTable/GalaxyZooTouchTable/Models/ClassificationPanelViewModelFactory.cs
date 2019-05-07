using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.ViewModels;
using System;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationPanelViewModelFactory : IClassificationPanelViewModelFactory
    {
        private ILocalDBService _localDBService;
        private IGraphQLService _graphQLService;
        private IPanoptesService _panoptesService;

        public ClassificationPanelViewModelFactory(IPanoptesService panoptesService, IGraphQLService graphQLService, ILocalDBService localDBService)
        {
            if (panoptesService == null || graphQLService == null || localDBService == null)
            {
                throw new ArgumentNullException("RepoDependency");
            }

            _localDBService = localDBService;
            _graphQLService = graphQLService; 
            _panoptesService = panoptesService;
        }

        public ClassificationPanelViewModel Create(UserType type)
        {
            TableUser User = AssignUser(type);
            return new ClassificationPanelViewModel(_panoptesService, _graphQLService, _localDBService, User);
        }

        private TableUser AssignUser(UserType type)
        {
            switch (type)
            {
                case UserType.Person: return GlobalData.GetInstance().PersonUser;
                case UserType.Star: return GlobalData.GetInstance().StarUser;
                case UserType.Earth: return GlobalData.GetInstance().EarthUser;
                case UserType.Light: return GlobalData.GetInstance().LightUser;
                case UserType.Face: return GlobalData.GetInstance().FaceUser;
                case UserType.Pink: return GlobalData.GetInstance().PinkUser;
                default: return GlobalData.GetInstance().PinkUser;
            }
        }
    }
}
