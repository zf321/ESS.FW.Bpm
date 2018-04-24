using System.Diagnostics;
using System.Threading;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    public class TweetExceptionHandler : IJobHandler<IJobHandlerConfiguration>
    {
        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        public const string TYPE = "tweet-exception";

        //protected internal AtomicInteger exceptionsRemaining = new AtomicInteger(2);
        protected int exceptionsRemaining = 2;

        public virtual int ExceptionsRemaining
        {
            get { return exceptionsRemaining; }
            set { exceptionsRemaining = value; }
        }


        public virtual string Type
        {
            get { return TYPE; }
        }

        public virtual void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            if (Interlocked.Decrement(ref exceptionsRemaining) >= 0)
                throw new System.Exception("exception remaining: " + ExceptionsRemaining);
            Debug.WriteLine("no more exceptions to throw.");
        }

        public IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            return new JobHandlerConfigurationAnonymousInnerClass();
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        private class JobHandlerConfigurationAnonymousInnerClass : IJobHandlerConfiguration
        {
            public string ToCanonicalString()
            {
                return null;
            }
        }
    }
}