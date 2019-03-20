using GalaxyZooTouchTable.Models;

namespace GalaxyZooTouchTable.Tests.Mock
{
    public static class SpaceNavigationMockData
    {
        public static SpacePoint Center()
        {
            return new SpacePoint(10, 10);
        }

        public static SpaceNavigation CurrentLocation()
        {
            return new SpaceNavigation(Center());
        }
    }
}
