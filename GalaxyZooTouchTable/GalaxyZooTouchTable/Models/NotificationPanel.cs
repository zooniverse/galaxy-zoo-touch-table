using GalaxyZooTouchTable.Lib;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public class NotificationPanel
    {
        public BitmapImage Avatar { get; private set; }
        public string Answer { get; private set; }
        public NotificationPanelStatus Status { get; set; }

        public NotificationPanel(NotificationPanelStatus status, BitmapImage avatar = null, string answer = null)
        {
            Answer = answer;
            Avatar = avatar;
            Status = status;
        }
    }
}
