using System;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class SetJobDuedateCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        private readonly string _jobId;
        private readonly DateTime _newDuedate;

        public SetJobDuedateCmd(string jobId, DateTime newDuedate)
        {
            if (ReferenceEquals(jobId, null) || (jobId.Length < 1))
                throw new ProcessEngineException("The job id is mandatory, but '" + jobId + "' has been provided.");
            this._jobId = jobId;
            this._newDuedate = newDuedate;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            JobEntity job = commandContext.JobManager.FindJobById(_jobId);
            if (job != null)
            {
                foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                {
                    checker.CheckUpdateJob(job);
                }

                job.Duedate = _newDuedate;
            }
            else
            {
                throw new ProcessEngineException("No job found with id '" + _jobId + "'.");
            }
            return null;
        }
    }
}