namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     holds the parameters of a page (partial result) for a query.
    ///     
    ///      
    /// </summary>
    public class Page
    {
        public Page(int firstResult, int maxResults)
        {
            this.FirstResult = firstResult;
            this.MaxResults = maxResults;
        }

        public virtual int FirstResult { get; }

        public virtual int MaxResults { get; }
    }
}