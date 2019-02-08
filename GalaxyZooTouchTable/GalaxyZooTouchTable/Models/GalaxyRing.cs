using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public class GalaxyRing
    {
        const int RING_WIDTH_STEP = 16;
        const int INITIAL_RING_WIDTH = 56;
        const int INITIAL_AVATAR_RADIUS_POSITION = 26;
        const int INITIAL_RADIUS = 28;

        public int Diameter { get; set; }
        public int CornerRadius { get; set; }
        public BitmapImage UserAvatar { get; set; }
        public int AvatarX { get; set; }
        public int AvatarY { get; set; }
        public string UserColor { get; set; }
        public int YPos { get; set; }

        public GalaxyRing(int index, TableUser user)
        {
            UserColor = user.ThemeColor;
            UserAvatar = user.Avatar;
            GetProperties(index);
        }

        private void GetProperties(int index)
        {
            Diameter = INITIAL_RING_WIDTH + (RING_WIDTH_STEP * index);
            CornerRadius = Diameter / 2;
            AvatarX = INITIAL_AVATAR_RADIUS_POSITION + (index * RING_WIDTH_STEP);
            YPos = index == 0 ? 0 : (Diameter) * -1;
            var test = ((INITIAL_RING_WIDTH * index) + Diameter) * -1;

            var position = 0;
            for (var start = 0; start < index; start++)
            {
                position -= (Diameter - (8 * index));
            }
            YPos = position;
        }
    }
}
