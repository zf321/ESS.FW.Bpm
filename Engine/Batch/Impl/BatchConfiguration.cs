using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{
    public  class BatchConfiguration: IJobHandlerConfiguration
    {
        public BatchConfiguration(IList<string> ids)
        {
            this.Ids = ids;
        }

        public IList<string> Ids { get; set; }
        public string ToCanonicalString()
        {
            return string.Empty;
        }
    }
}