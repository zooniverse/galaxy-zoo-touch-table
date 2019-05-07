using GalaxyZooTouchTable.ViewModels;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public class GalaxyRing : ViewModelBase
    {
        const int RING_WIDTH_STEP = 16;
        const int AVATAR_WIDTH_STEP = 6;
        const int INITIAL_RING_WIDTH = 56;
        const int INITIAL_AVATAR_RADIUS_POSITION = 20;

        public BitmapImage UserAvatar { get; set; }
        public string UserColor { get; set; } = "#E5FF4D";
        public string UserName { get; set; }
        public string UserDash { get; set; } = "1 0";

        private int _avatarX = 0;
        public int AvatarX
        {
            get => _avatarX;
            set => SetProperty(ref _avatarX, value);
        }

        private int _avatarY = 0;
        public int AvatarY
        {
            get => _avatarY;
            set => SetProperty(ref _avatarY, value);
        }

        private int _diameter = 0;
        public int Diameter
        {
            get => _diameter;
            set => SetProperty(ref _diameter, value);
        }

        private bool _currentlyClassifying = true;
        public bool CurrentlyClassifying
        {
            get => _currentlyClassifying;
            set => SetProperty(ref _currentlyClassifying, value);
        }

        public GalaxyRing(int index = 0, TableUser user = null)
        {
            if (user != null)
            {
                UserName = user.Name;
                UserColor = user.ThemeColor;
                UserAvatar = user.Avatar;
                UserDash = user.DashArray;
            }
            SetProperties(index);
        }

        public void SetProperties(int index)
        {
            Diameter = INITIAL_RING_WIDTH + (RING_WIDTH_STEP * index);
            AvatarX = INITIAL_AVATAR_RADIUS_POSITION + (index * (AVATAR_WIDTH_STEP));
            AvatarY = INITIAL_AVATAR_RADIUS_POSITION + (index * (AVATAR_WIDTH_STEP));

            PlaceAvatar();
        }

        private void PlaceAvatar()
        {
            if (UserName != null)
            {
                switch (UserName)
                {
                    case "PinkUser":
                        AvatarY *= -1;
                        AvatarX *= -1;
                        break;
                    case "StarUser":
                        AvatarY *= -1;
                        break;
                    case "LightUser":
                        AvatarY = 0;
                        AvatarX = Diameter / 2;
                        break;
                    case "PersonUser":
                        AvatarX *= -1;
                        break;
                    case "EarthUser":
                        AvatarY = 0;
                        AvatarX = Diameter / 2 * -1;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
