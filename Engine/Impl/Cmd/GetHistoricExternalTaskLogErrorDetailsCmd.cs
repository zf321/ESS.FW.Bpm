using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    public class GetHistoricExternalTaskLogErrorDetailsCmd : ICommand<string>
    {
        protected internal string HistoricExternalTaskLogId;

        public GetHistoricExternalTaskLogErrorDetailsCmd(string historicExternalTaskLogId)
        {
            this.HistoricExternalTaskLogId = historicExternalTaskLogId;
        }

        public virtual string Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("historicExternalTaskLogId", HistoricExternalTaskLogId);

            HistoricExternalTaskLogEntity @event =
                commandContext.HistoricExternalTaskLogManager.FindHistoricExternalTaskLogById(HistoricExternalTaskLogId);

            EnsureUtil.EnsureNotNull("No historic external ITask log found with id " + HistoricExternalTaskLogId,
                "historicExternalTaskLog", @event);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadHistoricExternalTaskLog(@event);
            }

            return @event.ErrorDetails;
        }
    }
}