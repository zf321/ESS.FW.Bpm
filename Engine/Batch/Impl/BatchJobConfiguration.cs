using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class BatchJobConfiguration : IJobHandlerConfiguration
    {
        protected internal string configurationByteArrayId;

        public BatchJobConfiguration(string configurationByteArrayId)
        {
            this.configurationByteArrayId = configurationByteArrayId;
        }

        public virtual string ConfigurationByteArrayId
        {
            get { return configurationByteArrayId; }
        }

        public virtual string ToCanonicalString()
        {
            return configurationByteArrayId;
        }
    }
}