using PanoptesNetClient.Models;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationRingNotifier
    {
        public TableUser User { get; set; }
        public Subject Subject { get; set; }
        public bool IsCreating { get; set; }

        public ClassificationRingNotifier(Subject subject, TableUser user, bool isCreating)
        {
            IsCreating = isCreating;
            Subject = subject;
            User = user;
        }
    }
}
