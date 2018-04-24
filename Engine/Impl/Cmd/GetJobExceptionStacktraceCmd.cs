using System;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetJobExceptionStacktraceCmd : ICommand<string>
    {
        private const long SerialVersionUid = 1L;
        private readonly string _jobId;

        public GetJobExceptionStacktraceCmd(string jobId)
        {
            this._jobId = jobId;
        }


        public virtual string Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("jobId", _jobId);

            JobEntity job = commandContext.JobManager.FindJobById(_jobId);

            EnsureUtil.EnsureNotNull("No job found with id " + _jobId, "job", job);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadJob(job);
            }

            return job.ExceptionStacktrace;
        }
    }
}