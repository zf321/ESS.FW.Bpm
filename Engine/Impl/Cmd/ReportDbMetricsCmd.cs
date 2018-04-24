using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class ReportDbMetricsCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        public virtual object Execute(CommandContext commandContext)
        {
            var engineConfiguration = context.Impl.Context.ProcessEngineConfiguration;

            if (!engineConfiguration.MetricsEnabled)
                throw new ProcessEngineException("Metrics reporting is disabled");

            if (!engineConfiguration.DbMetricsReporterActivate)
                throw new ProcessEngineException("Metrics reporting to database is disabled");

            var dbMetricsReporter = engineConfiguration.DbMetricsReporter;
            dbMetricsReporter.ReportNow();
            return null;
        }
    }
}