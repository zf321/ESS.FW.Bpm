using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class ExecuteJobsCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        protected internal JobFailureCollector JobFailureCollector;

        protected internal string JobId;

        public ExecuteJobsCmd(string jobId, JobFailureCollector jobFailureCollector)
        {
            this.JobId = jobId;
            this.JobFailureCollector = jobFailureCollector;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("jobId", JobId);
            
            JobEntity job = commandContext.JobManager.Get( JobId);
            
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            var identityService = processEngineConfiguration.IdentityService;
            
            var jobExecutorContext = context.Impl.Context.JobExecutorContext;

            if (job == null)
            {
                if (jobExecutorContext != null)
                {
                    Log.DebugAcquiredJobNotFound(JobId);
                }
                else
                {
                    throw Log.JobNotFoundException(JobId);
                }
            }

            JobFailureCollector.Job = job;

            if (jobExecutorContext == null)
                foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                {
                    checker.CheckUpdateJob(job);
                }

            try
            {
                // register as command context close lister to intercept exceptions on flush
                commandContext.RegisterCommandContextListener(JobFailureCollector);

                commandContext.CurrentJob = job;

                job.Execute(commandContext);
            }
            finally
            {
                if (jobExecutorContext != null)
                {
                    jobExecutorContext.CurrentJob = null;
                    identityService.ClearAuthentication();
                }
            }
            return null;
        }
    }
}