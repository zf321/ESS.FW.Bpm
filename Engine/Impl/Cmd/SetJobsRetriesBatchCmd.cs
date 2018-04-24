using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class SetJobsRetriesBatchCmd : AbstractSetJobsRetriesBatchCmd
    {
        protected internal readonly IList<string> jobIds;
        protected internal readonly Expression<Func<IJob,bool>> jobQuery;

        public SetJobsRetriesBatchCmd(IList<string> jobIds, Expression<Func<IJob,bool>> jobQuery, int retries)
        {
            this.jobQuery = jobQuery;
            this.jobIds = jobIds;
            base.Retries = retries;
        }

        public virtual IList<string> JobIds
        {
            get { return jobIds; }
        }
        

        public virtual Expression<Func<IJob,bool>> JobQuery
        {
            get { return jobQuery; }
        }

        protected internal override IList<string> CollectJobIds(CommandContext commandContext)
        {
            throw new NotImplementedException();
//            ISet<string> collectedJobIds = new HashSet<string>();

//            var jobIds = JobIds;
//            if (jobIds != null)
//            {
//                collectedJobIds.addAll(jobIds);
//            }

////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.engine.runtime.JobQuery jobQuery = this.jobQuery;
//            var jobQuery = this.jobQuery;
//            if (jobQuery != null)
//            {
//                foreach (IJob job in jobQuery.list())
//                {
//                    collectedJobIds.Add(job.Id);
//                }
//            }

//            return new List<string>(collectedJobIds);
        }

        protected internal override IBatchJobHandler GetBatchJobHandler(
            ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            throw new NotImplementedException();
        }
    }
}