

using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;

namespace ESS.FW.Bpm.Engine.Management.Impl
{
    /// <summary>
    ///     The purge report contains information about the deleted rows for each table
    ///     and also the deleted values which are removed from the deployment cache.
    ///     If now entities are deleted since the database was already clean the purge report is empty.
    ///      
    /// </summary>
    public class PurgeReport
    {
        public virtual DatabasePurgeReport DatabasePurgeReport { get; set; }

        


        public virtual CachePurgeReport CachePurgeReport { get; set; }


        //public virtual bool Empty
        //{
        //    get { return cachePurgeReport.Empty && databasePurgeReport.Empty; }
        //}
    }
}