using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Batch.Impl.Job
{
    /// <summary>
    ///     
    /// </summary>
    public class SetJobRetriesBatchConfiguration : BatchConfiguration
    {
        protected internal int retries;

        public SetJobRetriesBatchConfiguration(IList<string> ids, int retries) : base(ids)
        {
            this.retries = retries;
        }

        public virtual int Retries
        {
            get { return retries; }
            set { retries = value; }
        }
    }
}