using System;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    enum UserType
    {
        Person,
        Star,
        Earth,
        Light,
        Face,
        Heart
    }

    abstract class TableUser
    {
        public abstract string ThemeColor { get; }
        public abstract BitmapImage StartButton { get; }
    }

    static class TableUserFactory
    {
        public static TableUser Create(UserType type)
        {
            switch (type)
            {
                case UserType.Person:
                    return new PersonUser();
                case UserType.Star:
                    return new StarUser();
                case UserType.Earth:
                    return new EarthUser();
                case UserType.Light:
                    return new LightUser();
                case UserType.Face:
                    return new FaceUser();
                case UserType.Heart:
                    return new HeartUser();
                default:
                    return new HeartUser();

            }
        }

        private class PersonUser : TableUser
        {
            public override string ThemeColor => "#A5A2FB";
            public override BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Person.png", UriKind.Relative));
        }

        private class StarUser : TableUser
        {
            public override string ThemeColor => "#29A1FA";
            public override BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Star.png", UriKind.Relative));
        }

        private class EarthUser : TableUser
        {
            public override string ThemeColor => "#6ADCA3";
            public override BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Earth.png", UriKind.Relative));
        }

        private class LightUser : TableUser
        {
            public override string ThemeColor => "#A3DDEE";
            public override BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Light.png", UriKind.Relative));
        }

        private class FaceUser : TableUser
        {
            public override string ThemeColor => "#F3AB91";
            public override BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Face.png", UriKind.Relative));
        }

        private class HeartUser : TableUser
        {
            public override string ThemeColor => "#F3588B";
            public override BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Heart.png", UriKind.Relative));
        }
    }
}
