namespace GalaxyZooTouchTable.Lib
{
    /// <summary>
    /// IsCreating: User is beginning to classify a galaxy and a ring should be created
    /// IsSubmitting: User is submitting a classification and a ring should be dimmed
    /// IsHelping: User is changing subjects due to helping a neighbor and active ring should be changed
    /// IsLeaving: User is leaving the table and a ring should be removed
    /// </summary>
    public enum RingNotifierStatus
    {
        IsCreating,
        IsSubmitting,
        IsHelping,
        IsLeaving
    }
}
