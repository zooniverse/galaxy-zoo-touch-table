namespace GalaxyZooTouchTable.Models
{
    public class HelpNotification
    {
        public TableUser SentBy { get; private set; }
        public HelpNotificationStatus Status { get; private set; }
        public CompletedClassification Classification { get; private set; }

        public HelpNotification(TableUser sentBy, HelpNotificationStatus status, CompletedClassification classification = null)
        {
            Classification = classification;
            SentBy = sentBy;
            Status = status;
        }
    }

    public enum HelpNotificationStatus
    {
        AskForHelp,
        SendAnswer,
        Decline
    }
}
