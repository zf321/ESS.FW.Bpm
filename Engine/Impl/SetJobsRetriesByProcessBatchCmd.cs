using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class SetJobsRetriesByProcessBatchCmd : AbstractSetJobsRetriesBatchCmd
    {
        protected internal readonly IList<string> ProcessInstanceIds;
        protected internal readonly IQueryable<IProcessInstance> Query;

        public SetJobsRetriesByProcessBatchCmd(IList<string> processInstanceIds, IQueryable<IProcessInstance> query, int retries)
        {
            this.ProcessInstanceIds = processInstanceIds;
            this.Query = query;
            this.Retries = retries;
        }

        protected internal override IList<string> CollectJobIds(CommandContext commandContext)
        {
            IList<string> collectedJobIds = new List<string>();
            IList<string> collectedProcessInstanceIds = new List<string>();

            if (Query != null)
                ((List<string>) collectedProcessInstanceIds).AddRange(Query.Select(c=>c.Id));

            if (ProcessInstanceIds != null)
                ((List<string>) collectedProcessInstanceIds).AddRange(ProcessInstanceIds);

            foreach (var process in collectedProcessInstanceIds)
            {
                foreach (IJob job in commandContext.JobManager.FindJobsByProcessInstanceId(process))
                {
                    collectedJobIds.Add(job.Id);
                }
            }

            return collectedJobIds;
        }
        
    }
}