using GalaxyZooTouchTable.ViewModels;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public class GalaxyRing : ViewModelBase
    {
        const int RING_WIDTH_STEP = 16;
        const int INITIAL_RING_WIDTH = 56;
        const int INITIAL_AVATAR_RADIUS_POSITION = 20;
        const int INITIAL_RADIUS = 28;

        public int Diameter { get; set; }
        public int CornerRadius { get; set; }
        public BitmapImage UserAvatar { get; set; }
        public int AvatarX { get; set; }
        public int AvatarY { get; set; }
        public string UserColor { get; set; } = "#E5FF4D";
        public int YPos { get; set; }
        public TableUser User { get; private set; }

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
                User = user;
                UserColor = user.ThemeColor;
                UserAvatar = user.Avatar;
            }
            GetProperties(index);
        }

        private void GetProperties(int index)
        {
            Diameter = INITIAL_RING_WIDTH + (RING_WIDTH_STEP * index);
            CornerRadius = Diameter / 2;
            AvatarX = INITIAL_AVATAR_RADIUS_POSITION + (index * (6));
            AvatarY = INITIAL_AVATAR_RADIUS_POSITION + (index * (6));
            YPos = index == 0 ? 0 : (Diameter) * -1;

            AvatarRotation(index);

            var position = 0;
            for (var start = 0; start < index; start++)
            {
                position -= (Diameter - (8 * index));
            }
            YPos = position;
        }

        private void AvatarRotation(int index)
        {
            var RotationStart = index + 1;
            var Remainder = RotationStart % 4;

            switch (Remainder)
            {
                case 1:
                    AvatarX = AvatarX * -1;
                    break;
                case 2:
                    AvatarX = AvatarX * -1;
                    AvatarY = AvatarY * -1;
                    break;
                case 3:
                    AvatarY = AvatarY * -1;
                    break;
                default:
                    break;
            }
        }
    }
}
