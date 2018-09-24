using GalaxyZooTouchTable.Utility;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
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
        string Description2 { get; }
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
            public string Description => "Smooth galaxies fade gradually in all directions from the center";
            public string Description2 => "There may be a small bright symmetric core.";
        }

        private class FeaturesGalaxy : GalaxyExample
        {
            public BitmapImage ImageOne => new BitmapImage(new Uri("../Images/GalaxyExamples/features1.jpeg", UriKind.Relative));
            public BitmapImage ImageTwo => new BitmapImage(new Uri("../Images/GalaxyExamples/features2.jpg", UriKind.Relative));
            public BitmapImage ImageThree => new BitmapImage(new Uri("../Images/GalaxyExamples/features3.jpg", UriKind.Relative));
            public string Title => "Features or Disk";
            public string Description => "Galaxies might have spiral arms, a bulge or bar, or any other interesting feature.";
            public string Description2 => "Choose this option if you see anything unique about the galaxy";
        }

        private class StarGalaxy : GalaxyExample
        {
            public BitmapImage ImageOne => new BitmapImage(new Uri("../Images/GalaxyExamples/star1.jpg", UriKind.Relative));
            public BitmapImage ImageTwo => new BitmapImage(new Uri("../Images/GalaxyExamples/star2.jpg", UriKind.Relative));
            public BitmapImage ImageThree => new BitmapImage(new Uri("../Images/GalaxyExamples/smooth3.jpg", UriKind.Relative));
            public string Title => "Star or Artifact";
            public string Description => "Choose \"Star or Artifact\" if there is no central galaxy to classify or if the artifact is so badly displayed that you can't ignore it and classify the galaxy with reasonable confidence.";
            public string Description2 => "The telescopes taking our data were designed to look at faint galaxies, which means that bright, compact objects like stars sometimes look a bit strange.";
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
