﻿using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.ViewModels;
using System;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationPanelViewModelFactory : IClassificationPanelViewModelFactory
    {
        private ILocalDBService _localDBService;
        private IGraphQLService _graphQLService;

        public ClassificationPanelViewModelFactory(IGraphQLService graphQLService, ILocalDBService localDBService)
        {
            if (graphQLService == null || localDBService == null)
            {
                throw new ArgumentNullException("RepoDependency");
            }

            _localDBService = localDBService;
            _graphQLService = graphQLService; 
        }

        public ClassificationPanelViewModel Create(UserType type)
        {
            TableUser User = AssignUser(type);
            return new ClassificationPanelViewModel(new PanoptesService(_localDBService), _localDBService, User);
        }

        private TableUser AssignUser(UserType type)
        {
            switch (type)
            {
                case UserType.Purple: return GlobalData.GetInstance().PurpleUser;
                case UserType.Blue: return GlobalData.GetInstance().BlueUser;
                case UserType.Green: return GlobalData.GetInstance().GreenUser;
                case UserType.Aqua: return GlobalData.GetInstance().AquaUser;
                case UserType.Peach: return GlobalData.GetInstance().PeachUser;
                case UserType.Pink: return GlobalData.GetInstance().PinkUser;
                default: return GlobalData.GetInstance().PinkUser;
            }
        }
    }
}
