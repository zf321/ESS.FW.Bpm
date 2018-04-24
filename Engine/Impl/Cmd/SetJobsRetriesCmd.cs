using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class SetJobsRetriesCmd : AbstractSetJobRetriesCmd, ICommand<object>
    {
        protected internal readonly IList<string> JobIds;
        protected internal readonly int Retries;

        public SetJobsRetriesCmd(IList<string> jobIds, int retries)
        {
            EnsureUtil.EnsureNotEmpty("Job ID's", string.Join(",", jobIds));
            EnsureUtil.EnsureGreaterThanOrEqual("Retries count", retries, 0);

            this.JobIds = jobIds;
            this.Retries = retries;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            foreach (var id in JobIds)
                SetJobRetriesByJobId(id, Retries, commandContext);
            return null;
        }
    }
}