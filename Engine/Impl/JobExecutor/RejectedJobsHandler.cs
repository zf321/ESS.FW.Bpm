using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     <para>
    ///         Strategy for handling jobs that were acquired but cannot be
    ///         executed at this point (queue-size exceeded).
    ///     </para>
    ///     
    /// </summary>
    public interface IRejectedJobsHandler
    {
        void JobsRejected(IList<string> jobIds, ProcessEngineImpl processEngine, JobExecutor jobExecutor);
    }
}