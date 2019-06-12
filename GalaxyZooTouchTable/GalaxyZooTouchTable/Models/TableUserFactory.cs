using GalaxyZooTouchTable.ViewModels;
using GalaxyZooTouchTable.Lib;
using System;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public enum UserType
    {
        Purple,
        Blue,
        Green,
        Aqua,
        Peach,
        Pink
    }

    public abstract class TableUser : ViewModelBase
    {
        abstract public string Name { get; }
        abstract public string ThemeColor { get; }
        abstract public BitmapImage Avatar { get; }
        abstract public string DashArray { get; }
        abstract public VerticalPosition WorkspaceVerticalPosition { get; }
        abstract public HorizontalPosition WorkspaceHorizontalPosition { get; }

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

    public class PurpleUser : TableUser
    {
        public override string Name => "PurpleUser";
        public override string ThemeColor => "#A5A2FB";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/purple.png", UriKind.Relative));
        public override string DashArray => "0.25 1.5 2 1.5";
        public override VerticalPosition WorkspaceVerticalPosition => VerticalPosition.Bottom;
        public override HorizontalPosition WorkspaceHorizontalPosition => HorizontalPosition.Left;
    }

    public class BlueUser : TableUser
    {
        public override string Name => "BlueUser";
        public override string ThemeColor => "#29A1FA";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/blue.png", UriKind.Relative));
        public override string DashArray => "0.25 0.8";
        public override VerticalPosition WorkspaceVerticalPosition => VerticalPosition.Top;
        public override HorizontalPosition WorkspaceHorizontalPosition => HorizontalPosition.Right;
    }

    public class GreenUser : TableUser
    {
        public override string Name => "GreenUser";
        public override string ThemeColor => "#6ADCA3";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/green.png", UriKind.Relative));
        public override string DashArray => "3 2";
        public override VerticalPosition WorkspaceVerticalPosition => VerticalPosition.Middle;
        public override HorizontalPosition WorkspaceHorizontalPosition => HorizontalPosition.Left;
    }

    public class AquaUser : TableUser
    {
        public override string Name => "AquaUser";
        public override string ThemeColor => "#A3DDEE";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/aqua.png", UriKind.Relative));
        public override string DashArray => "0.5 2";
        public override VerticalPosition WorkspaceVerticalPosition => VerticalPosition.Middle;
        public override HorizontalPosition WorkspaceHorizontalPosition => HorizontalPosition.Right;
    }

    public class PeachUser : TableUser
    {
        public override string Name => "PeachUser";
        public override string ThemeColor => "#F3AB91";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/peach.png", UriKind.Relative));
        public override string DashArray => "0.05 1.35";
        public override VerticalPosition WorkspaceVerticalPosition => VerticalPosition.Bottom;
        public override HorizontalPosition WorkspaceHorizontalPosition => HorizontalPosition.Right;
    }

    public class PinkUser : TableUser
    {
        public override string Name => "PinkUser";
        public override string ThemeColor => "#F3588B";
        public override BitmapImage Avatar => new BitmapImage(new Uri("../Images/Avatars/pink.png", UriKind.Relative));
        public override string DashArray => "0.15 2.25";
        public override VerticalPosition WorkspaceVerticalPosition => VerticalPosition.Top;
        public override HorizontalPosition WorkspaceHorizontalPosition => HorizontalPosition.Left;
    }
}
