using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.ViewModels;
using System;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationPanelViewModelFactory : IClassificationPanelViewModelFactory
    {
        private IPanoptesRepository _repo;

        public ClassificationPanelViewModelFactory(IPanoptesRepository repo)
        {
            if (repo == null)
            {
                throw new ArgumentNullException("dependency");
            }

            _repo = repo;
        }

        public ClassificationPanelViewModel Create(UserType type)
        {
            switch (type)
            {
                case UserType.Person:
                    return new ClassificationPanelViewModel(_repo, CommonData.GetInstance().PersonUser);
                case UserType.Star:
                    return new ClassificationPanelViewModel(_repo, CommonData.GetInstance().StarUser);
                case UserType.Earth:
                    return new ClassificationPanelViewModel(_repo, CommonData.GetInstance().EarthUser);
                case UserType.Light:
                    return new ClassificationPanelViewModel(_repo, CommonData.GetInstance().LightUser);
                case UserType.Face:
                    return new ClassificationPanelViewModel(_repo, CommonData.GetInstance().FaceUser);
                case UserType.Heart:
                    return new ClassificationPanelViewModel(_repo, CommonData.GetInstance().HeartUser);
                default:
                    return new ClassificationPanelViewModel(_repo, CommonData.GetInstance().HeartUser);
            }
        }
    }
}
