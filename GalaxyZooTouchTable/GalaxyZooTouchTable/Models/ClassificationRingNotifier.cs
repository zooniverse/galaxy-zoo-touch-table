using GalaxyZooTouchTable.Lib;
using PanoptesNetClient.Models;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationRingNotifier
    {
        public TableUser User { get; private set; }
        public string SubjectId { get; private set; }
        public RingNotifierStatus Status { get; set; }

        public ClassificationRingNotifier(TableSubject subject, TableUser user, RingNotifierStatus status)
        {
            SubjectId = subject != null ? subject.Id : null;
            Status = status;
            User = user;
        }
    }
}
