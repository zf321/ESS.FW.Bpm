namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public interface IJobAcquisitionStrategy
    {
        long WaitTime { get; }

        void Reconfigure(JobAcquisitionContext context);

        int GetNumJobsToAcquire(string processEngine);
    }
}