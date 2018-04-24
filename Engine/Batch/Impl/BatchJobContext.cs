using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class BatchJobContext
    {
        public BatchJobContext(BatchEntity batchEntity, ResourceEntity configuration)
        {
            Batch = batchEntity;
            this.Configuration = configuration;
        }

        public virtual BatchEntity Batch { get; set; }


        public virtual ResourceEntity Configuration { get; set; }
    }
}