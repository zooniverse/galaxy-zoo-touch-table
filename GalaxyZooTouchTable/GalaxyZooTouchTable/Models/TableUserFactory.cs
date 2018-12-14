using GalaxyZooTouchTable.Lib;
using System;
using System.ComponentModel;
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

    public class TableUser : INotifyPropertyChanged
    {
        public virtual string Name { get; }
        string ThemeColor { get; }
        BitmapImage StartButton { get; }
        BitmapImage Avatar { get; }

        private NotificationStatus _status = NotificationStatus.Idle;
        public NotificationStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyRaised("Status");
            }
        }

        private bool _active = false;
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                OnPropertyRaised("Active");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
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
            public override string Name => "PersonUser";
            public string ThemeColor => "#A5A2FB";
            public BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Person.png", UriKind.Relative));
            public BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/Person.png", UriKind.Relative));
        }

        private class StarUser : TableUser
        {
            public override string Name => "StarUser";
            public string ThemeColor => "#29A1FA";
            public BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Star.png", UriKind.Relative));
            public BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/Star.png", UriKind.Relative));
        }

        private class EarthUser : TableUser
        {
            public override string Name => "EarthUser";
            public string ThemeColor => "#6ADCA3";
            public BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Earth.png", UriKind.Relative));
            public BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/Earth.png", UriKind.Relative));
        }

        private class LightUser : TableUser
        {
            public override string Name => "LightUser";
            public string ThemeColor => "#A3DDEE";
            public BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Light.png", UriKind.Relative));
            public BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/Light.png", UriKind.Relative));
        }

        private class FaceUser : TableUser
        {
            public override string Name => "FaceUser";
            public string ThemeColor => "#F3AB91";
            public BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Face.png", UriKind.Relative));
            public BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/Face.png", UriKind.Relative));
        }

        private class HeartUser : TableUser
        {
            public override string Name => "HeartUser";
            public string ThemeColor => "#F3588B";
            public BitmapImage StartButton => new BitmapImage(new Uri("../Images/StartIcon/Heart.png", UriKind.Relative));
            public BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/Heart.png", UriKind.Relative));
        }
    }
}
