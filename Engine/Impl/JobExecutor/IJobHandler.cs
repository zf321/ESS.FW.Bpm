using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    /// </summary>
    public interface IJobHandler<T>: IJobHandler where T : IJobHandlerConfiguration
    {
    }

    public interface IJobHandler
    {
        string Type { get; }

        void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId);

        IJobHandlerConfiguration NewConfiguration(string canonicalString);

        /// <summary>
        ///     Clean up before job is deleted. Like removing of auxiliary entities specific for this job handler.
        /// </summary>
        /// <param name="configuration"> the job handler configuration </param>
        /// <param name="jobEntity"> the job entity to be deleted </param>
        void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity);
    }
}