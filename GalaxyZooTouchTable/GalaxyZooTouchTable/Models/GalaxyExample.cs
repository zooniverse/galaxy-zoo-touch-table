using System;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public enum GalaxyType
    {
        Smooth,
        Features,
        Star
    }

    public class GalaxyExample
    {
        BitmapImage ImageOne { get; }
        BitmapImage ImageTwo { get; set; }
        BitmapImage ImageThree { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Description2 { get; set; }
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
            public BitmapImage ImageOne => new BitmapImage(new Uri("pack://application:,,,/Images/GalaxyExamples/smooth1.jpg"));
            public BitmapImage ImageTwo => new BitmapImage(new Uri("pack://application:,,,/Images/GalaxyExamples/smooth2.jpg"));
            public BitmapImage ImageThree => new BitmapImage(new Uri("pack://application:,,,/Images/GalaxyExamples/smooth3.jpg"));
            public string Title => "Smooth";
            public string Description => "Smooth galaxies fade gradually in all directions from the center";
            public string Description2 => "There may be a small bright symmetric core.";
        }

        private class FeaturesGalaxy : GalaxyExample
        {
            public BitmapImage ImageOne => new BitmapImage(new Uri("pack://application:,,,/Images/GalaxyExamples/features1.jpeg"));
            public BitmapImage ImageTwo => new BitmapImage(new Uri("pack://application:,,,/Images/GalaxyExamples/features2.jpg"));
            public BitmapImage ImageThree => new BitmapImage(new Uri("pack://application:,,,/Images/GalaxyExamples/features3.jpg"));
            public string Title => "Features";
            public string Description => "Galaxies might have spiral arms, a bulge or bar, or any other interesting feature.";
            public string Description2 => "Choose this option if you see anything unique about the galaxy";
        }

        private class StarGalaxy : GalaxyExample
        {
            public BitmapImage ImageOne => new BitmapImage(new Uri("pack://application:,,,/Images/GalaxyExamples/star1.jpg"));
            public BitmapImage ImageTwo => new BitmapImage(new Uri("pack://application:,,,/Images/GalaxyExamples/star2.jpg"));
            public BitmapImage ImageThree => new BitmapImage(new Uri("pack://application:,,,/Images/GalaxyExamples/smooth3.jpg"));
            public string Title => "Not a Galaxy";
            public string Description => "Choose \"Star or Artifact\" if there is no central galaxy to classify or if the artifact is so badly displayed that you can't ignore it and classify the galaxy with reasonable confidence.";
            public string Description2 => "The telescopes taking our data were designed to look at faint galaxies, which means that bright, compact objects like stars sometimes look a bit strange.";
        }
    }
}
