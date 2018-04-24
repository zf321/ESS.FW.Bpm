using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    /// job故障信息
    /// </summary>
    public class JobFailureCollector : ICommandContextListener
    {
        protected internal System.Exception failure;
        protected internal JobEntity job;
        protected internal string jobId;

        public JobFailureCollector(string jobId)
        {
            this.jobId = jobId;
        }

        public virtual System.Exception Failure
        {
            set
            {
                // log value if not already present
                if (failure == null)
                    failure = value;
            }
            get { return failure; }
        }

        public virtual JobEntity Job
        {
            set { job = value; }
            get { return job; }
        }


        public virtual string JobId
        {
            get { return jobId; }
        }


        public virtual void OnCommandFailed(CommandContext commandContext, System.Exception t)
        {
            Failure = t;
        }

        public virtual void OnCommandContextClose(CommandContext commandContext)
        {
            // ignore
        }
    }
}