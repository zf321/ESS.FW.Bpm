using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{

    /// <summary>
    ///     Deletes historic decision instances with the given id of the instance.
    /// </summary>
    public class DeleteHistoricDecisionInstanceByInstanceIdCmd : ICommand<object>
    {
        protected internal readonly string HistoricDecisionInstanceId;

        public DeleteHistoricDecisionInstanceByInstanceIdCmd(string historicDecisionInstanceId)
        {
            this.HistoricDecisionInstanceId = historicDecisionInstanceId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("historicDecisionInstanceId", HistoricDecisionInstanceId);

            //IHistoricDecisionInstance historicDecisionInstance =
            //    commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstance(historicDecisionInstanceId);
            //EnsureUtil.EnsureNotNull("No historic decision instance found with id: " + historicDecisionInstanceId,
            //    "historicDecisionInstance", historicDecisionInstance);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                //checker.checkDeleteHistoricDecisionInstance(historicDecisionInstance);
            }

            //commandContext.HistoricDecisionInstanceManager.deleteHistoricHistoricInstanceByInstanceId(
            //    historicDecisionInstanceId);

            return null;
        }
    }
}