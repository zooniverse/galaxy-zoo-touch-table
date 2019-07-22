using GalaxyZooTouchTable.Lib;

namespace GalaxyZooTouchTable.Tests.Mock
{
    public static class CutoutServiceMockData
    {
        public static SpaceCutout SpaceCutout()
        {
            SpaceCutout cutout = new SpaceCutout();
            cutout.ImageOne = new System.Windows.Media.Imaging.BitmapImage();
            return cutout;
        }
    }
}
