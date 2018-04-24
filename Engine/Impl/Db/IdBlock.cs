namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     
    /// </summary>
    public class IdBlock
    {
        public IdBlock(long nextId, long lastId)
        {
            NextId = nextId;
            this.LastId = lastId;
        }

        public virtual long NextId { get; }

        public virtual long LastId { get; }
    }
}