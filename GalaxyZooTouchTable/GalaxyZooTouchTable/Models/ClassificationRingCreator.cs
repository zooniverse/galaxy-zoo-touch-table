using PanoptesNetClient.Models;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationRingCreator
    {
        public TableUser User { get; set; }
        public Subject Subject { get; set; }

        public ClassificationRingCreator(Subject subject, TableUser user)
        {
            Subject = subject;
            User = user;
        }
    }
}
