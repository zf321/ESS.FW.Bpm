namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     Entities that are using revisions for optimistic locking, need to implement this interface.
    ///     
    /// </summary>
    public interface IHasDbRevision
    {
        int Revision { set; get; }
        int RevisionNext { get; }
    }
}