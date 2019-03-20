using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public class NotificationOverlay
    {
        public BitmapImage Avatar { get; private set; }
        public string MessageOne { get; private set; }
        public string MessageTwo { get; private set; }

        public NotificationOverlay(string messageOne, string messageTwo = null, BitmapImage avatar = null)
        {
            Avatar = avatar;
            MessageOne = messageOne;
            MessageTwo = messageTwo;
        }
    }
}
