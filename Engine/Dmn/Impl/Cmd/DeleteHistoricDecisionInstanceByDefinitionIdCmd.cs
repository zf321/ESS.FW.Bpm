using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{

    /// <summary>
    ///     Deletes historic decision instances with the given id of the decision definition.
    ///     
    /// </summary>
    public class DeleteHistoricDecisionInstanceByDefinitionIdCmd : ICommand<object>
    {
        protected internal readonly string DecisionDefinitionId;

        public DeleteHistoricDecisionInstanceByDefinitionIdCmd(string decisionDefinitionId)
        {
            this.DecisionDefinitionId = decisionDefinitionId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("decisionDefinitionId", DecisionDefinitionId);

            var decisionDefinition =
                commandContext.DecisionDefinitionManager.FindDecisionDefinitionById(DecisionDefinitionId);
            EnsureUtil.EnsureNotNull("No decision definition found with id: " + DecisionDefinitionId,
                "decisionDefinition", decisionDefinition);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckDeleteHistoricDecisionInstance(decisionDefinition.Key);

            //commandContext.HistoricDecisionInstanceManager.deleteHistoricDecisionInstancesByDecisionDefinitionId(
            //    decisionDefinitionId);

            return null;
        }
    }
}