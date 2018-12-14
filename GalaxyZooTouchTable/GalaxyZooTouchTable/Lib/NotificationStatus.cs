namespace GalaxyZooTouchTable.Lib
{
    /// <summary>
    /// User needing help:
    /// DeclinedHelp: Shows in center that cooperating user has declined to help
    /// AcceptedHelp: Shows in center that cooperating user has accepted help
    /// AnswerGiven: Shows in center that cooperating user has answered and answer in left panel
    /// HelpRequestSent: Shows in center that request was sent
    /// PeerHasLeft: Shows in center that peer has left the table
    /// 
    /// User helping:
    /// HelpRequestReceived: Opens panel with accept or decline buttons
    /// HelpingUser: Shows circle around user that is being helped
    /// </summary>
    public enum NotificationStatus
    {
        Idle,
        HelpRequestReceived,
        DeclinedHelp,
        AcceptedHelp,
        HelpRequestSent,
        HelpingUser,
        AnswerGiven,
        PeerHasLeft
    }
}
