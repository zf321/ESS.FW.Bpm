using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public interface IAcquireJobsCommandFactory
    {
        ICommand<AcquiredJobs> GetCommand(int numJobsToAcquire);
    }
}