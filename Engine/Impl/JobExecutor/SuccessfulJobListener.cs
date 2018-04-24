using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public class SuccessfulJobListener : ICommand<object>
    {
        public virtual object Execute(CommandContext commandContext)
        {
            LogJobSuccess(commandContext);

            return null;
        }

        protected internal virtual void LogJobSuccess(CommandContext commandContext)
        {
            if (commandContext.ProcessEngineConfiguration.MetricsEnabled)
                commandContext.ProcessEngineConfiguration.MetricsRegistry.MarkOccurrence(Engine.Management.Metrics.JobSuccessful);
        }
    }
}