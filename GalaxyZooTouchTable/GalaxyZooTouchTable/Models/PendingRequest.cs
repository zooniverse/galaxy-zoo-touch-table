namespace GalaxyZooTouchTable.Models
{
    public class PendingRequest
    {
        public TableUser CooperatingPeer { get; private set; }
        public bool Assisting { get; private set; }
        public string SubjectId { get; private set; }

        public PendingRequest(TableUser cooperatingPeer, bool assisting, string subjectId)
        {
            Assisting = assisting;
            CooperatingPeer = cooperatingPeer;
            SubjectId = subjectId;
        }
    }
}
