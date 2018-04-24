using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    /// <summary>
    ///     
    /// </summary>
    public class GetHistoricJobLogExceptionStacktraceCmd : ICommand<string>
    {
        protected internal string HistoricJobLogId;

        public GetHistoricJobLogExceptionStacktraceCmd(string historicJobLogId)
        {
            this.HistoricJobLogId = historicJobLogId;
        }

        public virtual string Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("historicJobLogId", HistoricJobLogId);

            HistoricJobLogEventEntity job = commandContext.HistoricJobLogManager.FindHistoricJobLogById(HistoricJobLogId);

            EnsureUtil.EnsureNotNull("No historic job log found with id " + HistoricJobLogId, "historicJobLog", job);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadHistoricJobLog(job);
            }

            return job.ExceptionStacktrace;
        }
    }
}