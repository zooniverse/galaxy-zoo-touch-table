using GalaxyZooTouchTable.Lib;
using PanoptesNetClient.Models;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationRingNotifier
    {
        public TableUser User { get; private set; }
        public string SubjectId { get; private set; }
        public RingNotifierStatus Status { get; set; }

        public ClassificationRingNotifier(Subject subject, TableUser user, RingNotifierStatus status)
        {
            Status = status;
            SubjectId = subject.Id;
            User = user;
        }
    }
}
