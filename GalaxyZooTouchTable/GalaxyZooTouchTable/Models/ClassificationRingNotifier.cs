using GalaxyZooTouchTable.Lib;
using PanoptesNetClient.Models;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationRingNotifier
    {
        public TableUser User { get; private set; }
        public Subject Subject { get; private set; }
        public RingNotifierStatus Status { get; set; }

        public ClassificationRingNotifier(Subject subject, TableUser user, RingNotifierStatus status)
        {
            Status = status;
            Subject = subject;
            User = user;
        }
    }
}
