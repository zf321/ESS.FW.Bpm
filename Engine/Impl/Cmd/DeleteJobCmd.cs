using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    [Serializable]
    public class DeleteJobCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal string JobId;

        public DeleteJobCmd(string jobId)
        {
            this.JobId = jobId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("jobId", JobId);

            JobEntity job = commandContext.JobManager.FindJobById(JobId);
            EnsureUtil.EnsureNotNull("No job found with id '" + JobId + "'", "job", job);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckUpdateJob(job);
            }
            // We need to check if the job was locked, ie acquired by the job acquisition thread
            // This happens if the the job was already acquired, but not yet executed.
            // In that case, we can't allow to delete the job.
            if (!ReferenceEquals(job.LockOwner, null) || job.LockExpirationTime != null)
            {
                throw new ProcessEngineException("Cannot delete job when the job is being executed. Try again later.");
            }

            job.Delete();
            return null;
        }
    }
}