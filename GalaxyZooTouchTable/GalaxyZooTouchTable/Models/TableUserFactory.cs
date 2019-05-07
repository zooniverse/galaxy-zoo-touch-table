using GalaxyZooTouchTable.ViewModels;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public enum UserType
    {
        Person,
        Star,
        Earth,
        Light,
        Face,
        Heart
    }

    public abstract class TableUser : ViewModelBase
    {
        abstract public string Name { get; }
        abstract public string ThemeColor { get; }
        abstract public BitmapImage Avatar { get; }
        abstract public string DashArray { get; }

        private bool _busy = false;
        public bool Busy
        {
            get => _busy;
            set => SetProperty(ref _busy, value);
        }

        private bool _active = false;
        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }
    }

    public class PersonUser : TableUser
    {
        public override string Name => "PersonUser";
        public override string ThemeColor => "#A5A2FB";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/purple.png", UriKind.Relative));
        public override string DashArray => "0.25 1.5 2 1.5";
    }

    public class StarUser : TableUser
    {
        public override string Name => "StarUser";
        public override string ThemeColor => "#29A1FA";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/blue.png", UriKind.Relative));
        public override string DashArray => "0.25 0.8";
    }

    public class EarthUser : TableUser
    {
        public override string Name => "EarthUser";
        public override string ThemeColor => "#6ADCA3";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/green.png", UriKind.Relative));
        public override string DashArray => "3 2";
    }

    public class LightUser : TableUser
    {
        public override string Name => "LightUser";
        public override string ThemeColor => "#A3DDEE";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/aqua.png", UriKind.Relative));
        public override string DashArray => "0.5 2";
    }

    public class FaceUser : TableUser
    {
        public override string Name => "FaceUser";
        public override string ThemeColor => "#F3AB91";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/peach.png", UriKind.Relative));
        public override string DashArray => "0.05 1.35";
    }

    public class HeartUser : TableUser
    {
        public override string Name => "HeartUser";
        public override string ThemeColor => "#F3588B";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/pink.png", UriKind.Relative));
        public override string DashArray => "0.15 2.25";
    }
}
