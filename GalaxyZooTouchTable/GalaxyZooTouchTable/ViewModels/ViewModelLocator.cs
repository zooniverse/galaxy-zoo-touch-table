namespace GalaxyZooTouchTable.ViewModels
{
    public class ViewModelLocator
    {
        private static SpaceViewModel spaceViewModel
            = new SpaceViewModel();

        public static SpaceViewModel SpaceViewModel
        {
            get
            {
                return spaceViewModel;
            }
        }
    }
}
