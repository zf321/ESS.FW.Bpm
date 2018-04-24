using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class DeleteJobsCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal bool Cascade;

        protected internal IList<string> JobIds;

        public DeleteJobsCmd(IList<string> jobIds) : this(jobIds, false)
        {
        }

        public DeleteJobsCmd(IList<string> jobIds, bool cascade)
        {
            this.JobIds = jobIds;
            this.Cascade = cascade;
        }

        public DeleteJobsCmd(string jobId) : this(jobId, false)
        {
        }

        public DeleteJobsCmd(string jobId, bool cascade)
        {
            JobIds = new List<string>();
            JobIds.Add(jobId);
            this.Cascade = cascade;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            JobEntity jobToDelete = null;
            foreach (var jobId in JobIds)
            {
                jobToDelete = context.Impl.Context.CommandContext.JobManager.FindJobById(jobId);

                if (jobToDelete != null)
                {
                    // When given job doesn't exist, ignore
                    jobToDelete.Delete();

                    if (Cascade)
                    {
                        commandContext.HistoricJobLogManager.DeleteHistoricJobLogByJobId(jobId);
                    }
                }
            }
            return null;
        }
    }
}