using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public class DefaultAcquireJobsCommandFactory : IAcquireJobsCommandFactory
    {
        protected internal JobExecutor JobExecutor;

        public DefaultAcquireJobsCommandFactory(JobExecutor jobExecutor)
        {
            this.JobExecutor = jobExecutor;
        }

        public virtual ICommand<AcquiredJobs> GetCommand(int numJobsToAcquire)
        {
            return new AcquireJobsCmd(JobExecutor, numJobsToAcquire);
        }
    }
}