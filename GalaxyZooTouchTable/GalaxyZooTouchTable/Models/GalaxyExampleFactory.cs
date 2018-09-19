using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    enum GalaxyType
    {
        Smooth,
        Features,
        Star
    }

    public class GalaxyExample
    {
        BitmapImage ImageOne { get; }
        BitmapImage ImageTwo { get; }
        BitmapImage ImageThree { get; }
        string Title { get; }
        string Description { get; }
    }

    static class GalaxyExampleFactory
    {
        public static GalaxyExample Create(GalaxyType type)
        {
            switch (type)
            {
                case GalaxyType.Smooth:
                    return new SmoothGalaxy();
                case GalaxyType.Features:
                    return new FeaturesGalaxy();
                case GalaxyType.Star:
                    return new StarGalaxy();
                default:
                    return new StarGalaxy();
            }
        }

        private class SmoothGalaxy : GalaxyExample
        {
            public BitmapImage ImageOne => new BitmapImage(new Uri("../Images/GalaxyExamples/smooth1.jpg", UriKind.Relative));
            public BitmapImage ImageTwo => new BitmapImage(new Uri("../Images/GalaxyExamples/smooth2.jpg", UriKind.Relative));
            public BitmapImage ImageThree => new BitmapImage(new Uri("../Images/GalaxyExamples/smooth3.jpg", UriKind.Relative));
            public string Title => "Smooth";
            public string Description => throw new NotImplementedException();
        }

        private class FeaturesGalaxy : GalaxyExample
        {
            public BitmapImage ImageOne => new BitmapImage(new Uri("../Images/GalaxyExamples/features1.jpeg", UriKind.Relative));
            public BitmapImage ImageTwo => new BitmapImage(new Uri("../Images/GalaxyExamples/features2.jpg", UriKind.Relative));
            public BitmapImage ImageThree => new BitmapImage(new Uri("../Images/GalaxyExamples/features3.jpg", UriKind.Relative));
            public string Title => "Features or Disk";
            public string Description => throw new NotImplementedException();
        }

        private class StarGalaxy : GalaxyExample
        {
            public BitmapImage ImageOne => new BitmapImage(new Uri("../Images/GalaxyExamples/star1.jpg", UriKind.Relative));
            public BitmapImage ImageTwo => new BitmapImage(new Uri("../Images/GalaxyExamples/star2.jpg", UriKind.Relative));
            public BitmapImage ImageThree => new BitmapImage(new Uri("../Images/GalaxyExamples/smooth3.jpg", UriKind.Relative));
            public string Title => "Star or Artifact";
            public string Description => "If you can see any interesting features at all in the central falaxy, or if you think it is an edge-on disk, click \"Features or Disk.\"";
        }

        private async static Task<BitmapImage> GetBitmap(string filename)
        {
            Task<BitmapImage> t = Task.Run(() => {
                BitmapImage myBitmapImage = new BitmapImage();

                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(filename, UriKind.Relative);

                myBitmapImage.DecodePixelWidth = 200;
                myBitmapImage.EndInit();

                return myBitmapImage;
            });

            await t;
            return t.Result;
        }
    }
}
